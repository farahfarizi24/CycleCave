#!/usr/bin/env python3

import os
import sys
import gatt
import json
import time
import datetime
import logging
import re
import socket
import RPi.GPIO as GPIO

os.chdir(os.path.dirname(os.path.realpath(__file__)))

# setup logging
logger = logging.getLogger(__name__)
logger.setLevel(logging.INFO)
logger_formatter = logging.Formatter('%(levelname)s:%(name)s:%(message)s')
logger_file_handler = logging.FileHandler('cavebike.log')
logger_file_handler.setFormatter(logger_formatter)
logger_stream_handler = logging.StreamHandler()

logger.addHandler(logger_file_handler)
logger.addHandler(logger_stream_handler)

def handle_exception(exc_type, exc_value, exc_traceback):
    if issubclass(exc_type, KeyboardInterrupt):
        sys.__excepthook__(exc_type, exc_value, exc_traceback)
        return
    logger.critical("Uncaught exception", exc_info=(exc_type, exc_value, exc_traceback))

sys.excepthook = handle_exception

logger.info(f'Session Start:{datetime.datetime.now()}')

# define IP & Port
IP = '10.148.112.66' # PC's
#IP = '10.141.12.184' # D's
PORT = 5005
#IP = '10.141.30.208' # E's
#PORT = 1567

# constants
KICKR_ADDRESS = 'd2:83:84:53:44:47'
FTMS_UUID = 0x1826
INDOOR_BIKE_DATA_UUID = 0x2AD2
BRAKE_PIN = 31
MAGNETIC_BRAKE_PIN = 37

# data variables
speed = 0.0
cadence = 0.0
power = 0
brake = False

# helper function for matching uuids
def service_or_characteristic_found(target_uuid, full_uuid):
    uuid_string = hex(target_uuid)[2:]

    # assume the full uuid for a FTMS service or its characteristic will be like: "00002ad9-0000-1000-8000-00805f9b34fb"
    return bool(re.search(f"0000{uuid_string}", full_uuid, re.IGNORECASE))

# ===== Driver =====

class Cavebike(gatt.Device):

    def __init__(self, mac_address: str, manager: gatt.DeviceManager, sock: socket.socket, managed=True):
        super().__init__(mac_address, manager, managed)

        logger.info('Cavebike initialising')

        # define services/characteristics
        self.ftms = None
        self.indoor_bike_data = None

        # define socket
        self.sock = sock

    def connect_succeeded(self):
        logger.info('Connected to KICKR')

    def connect_failed(self, error):
        super().connect_failed(error)
        logger.info('Connection attempt failed')
        self.connect()

    def disconnect_succeeded(self):
        """When connection property changes and is no longer connected this method is called"""
        super().disconnect_succeeded()
        logger.info('Disconnected - attempting reconnect')
        self.connect()

    def set_service_or_characteristic(self, service_or_characteristic):

        if service_or_characteristic_found(FTMS_UUID, service_or_characteristic.uuid):
            self.ftms = service_or_characteristic
            logger.debug('FTMS service found')
        
        elif service_or_characteristic_found(INDOOR_BIKE_DATA_UUID, service_or_characteristic.uuid):
            self.indoor_bike_data = service_or_characteristic
            self.indoor_bike_data.enable_notifications()
            logger.debug('Indoor Bike Data characteristic found')

    # called when the bike data updates
    def characteristic_value_updated(self, characteristic, value):
        global speed, cadence, power, brake

        if not (characteristic == self.indoor_bike_data): return

        self.time_last_updated = time.time()

        logger.debug('Indoor Bike Data updated')

        # sanity check
        if value[0] != 68: raise Exception('Unexpected data present: speed, cadence and power should always & only be present.')     

        # extract speed data
        offset = 2
        speed = float((value[offset+1] << 8) + value[offset]) / 100.0 * 5.0 / 18.0

        # extract cadence data
        offset += 2
        cadence = float((value[offset+1] << 8) + value[offset]) / 10.0

        # extract power data
        offset += 2
        power = int((value[offset+1] << 8) + value[offset])

        # send data over UDP
        data = json.dumps({
            'ts' : time.time(), 
            'speed' : speed,
            'cadence' : cadence,
            'power' : power,
            'brake' : brake
        })
        self.sock.sendto(data.encode(), (IP, PORT))
        logger.debug(data)

    def services_resolved(self):
        super().services_resolved()

        logger.debug('Resolving services')

        for service in self.services:
            self.set_service_or_characteristic(service)
            for characteristic in service.characteristics:
                self.set_service_or_characteristic(characteristic)

        logger.debug('Services resolved')
        logger.info('Cavebike initialised')

class BrakeButton:

    def __init__(self, sock: socket.socket, pin: int = BRAKE_PIN, invert: bool = False):

        # setup pin
        self._pin = pin
        self._state = False
        self._invert = invert
        GPIO.setup(self._pin, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)
        GPIO.add_event_detect(self._pin, GPIO.BOTH, callback=self.state_change, bouncetime=75)

        # define socket
        self.sock = sock

        logger.info('Brake Button Initialised')

    def state_change(self, pin: int):
        global speed, cadence, power, brake
        
        self._state = bool(GPIO.input(self._pin))
        if self._invert: self._state = not self._state
        brake = self._state

        # send data over UDP
        #data = json.dumps({ 'ts' : time.time(), 'state' : self._state })
        data = json.dumps({
            'ts' : time.time(), 
            'speed' : speed,
            'cadence' : cadence,
            'power' : power,
            'brake' : brake
        })
        self.sock.sendto(data.encode(), (IP, PORT))
        logger.info(data)

def main():

    # setup socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    logger.info(f'Expected client is {IP}:{PORT}')

    # initialise brake button
    GPIO.setmode(GPIO.BOARD)
    brake_button = BrakeButton(sock,pin=BRAKE_PIN)
    magnetic_brake = BrakeButton(sock,pin=MAGNETIC_BRAKE_PIN,invert=False)

    # initialise the device manager
    manager = gatt.DeviceManager(adapter_name='hci0')
    manager.start_discovery()

    # initialise and connect to KICKR
    kickr = Cavebike(KICKR_ADDRESS, manager, sock)
    logger.info('Connecting to KICKR')
    kickr.connect()
    manager.stop_discovery()

    try:
        print("Running the device manager now...")
        # run the device manager in the main thread forever
        manager.run()
    except KeyboardInterrupt:
        print('Exit the program.')
        manager.stop()
        sock.close()
        sys.exit(0)

if __name__ == '__main__':
    main()