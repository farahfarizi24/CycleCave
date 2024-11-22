#!/usr/bin/env python3

import sys
import gatt
import json
import time
import logging
import re
import socket

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

# define IP & Port
IP = '10.141.12.184' #'10.148.112.66'
PORT = 5005

# constants
KICKR_ADDRESS = 'd2:83:84:53:44:47'
FTMS_UUID = 0x1826
INDOOR_BIKE_DATA_UUID = 0x2AD2

# helper function for matching uuids
def service_or_characteristic_found(target_uuid, full_uuid):
    uuid_string = hex(target_uuid)[2:]

    # assume the full uuid for a FTMS service or its characteristic will be like: "00002ad9-0000-1000-8000-00805f9b34fb"
    return bool(re.search(f"0000{uuid_string}", full_uuid, re.IGNORECASE))

# ===== Driver =====

class Cavebike(gatt.Device):

    def __init__(self, mac_address, manager, sock, managed=True):
        super().__init__(mac_address, manager, managed)

        logger.info('Cavebike initialising')

        # define services/characteristics
        self.ftms = None
        self.indoor_bike_data = None

        # define socket
        self.sock = sock

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

        if not (characteristic == self.indoor_bike_data): return

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
            'power' : power
        })
        self.sock.sendto(data.encode(), (IP, PORT))
        logger.info(data)

    def services_resolved(self):
        super().services_resolved()

        logger.debug('Resolving services')

        for service in self.services:
            self.set_service_or_characteristic(service)
            for characteristic in service.characteristics:
                self.set_service_or_characteristic(characteristic)

        logger.debug('Services resolved')
        logger.info('Cavebike initialised')

def main():

    # setup socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    logger.info(f'Expected client is {IP}:{PORT}')

    # initialise the device manager
    manager = gatt.DeviceManager(adapter_name='hci0')
    manager.start_discovery()

    # initialise and connect to KICKR
    kickr = Cavebike(KICKR_ADDRESS, manager, sock)
    logger.info('connecting to KICKR')
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