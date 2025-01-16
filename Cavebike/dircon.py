import socket
import threading
from zeroconf import ServiceBrowser, ServiceListener, Zeroconf

"""
Notes:
    - Possibility that zeroconf will not work due to compatibility issues.
To DO:
    - Test MDNS
    - Receive GATT data
    - Attempt direct indication and data processing
    - Automatically reconnect if DIRCON disconnects/shutsdown
"""

class MDNSListener(ServiceListener):

    def update_service(self, zc: Zeroconf, type_: str, name: str) -> None:
        print(f"Service {name} updated")

    def remove_service(self, zc: Zeroconf, type_: str, name: str) -> None:
        print(f"Service {name} removed")

    def add_service(self, zc: Zeroconf, type_: str, name: str) -> None:
        info = zc.get_service_info(type_, name)
        print(f"Service {name} added, service info: {info}")

class DIRCON:

    def __init__(self, sock: socket.socket, ip: str):
        self.sock = sock
        self.recvdata_thread = threading.Thread(target=self.recvdata)
        self.recvdata_thread.start() # FIXME: might need to move this to starting after connection
        self.sock.connect((ip, 36866))
        """Can we directly request indication on the Indoor Exercise Bike data characteristic without having to engage in anything else?"""

    def recvdata(self):
        while True:
            data = self.sock.recv(1024)
            print(data)

def main():

    try:
        # Discovery DIRCON IP address using MDNS
        zeroconf = Zeroconf()
        listener = MDNSListener()
        browser = ServiceBrowser(zeroconf, "_wahoo-fitness-tnp._tcp.local", listener)
        """Review listener output data to extract IP address"""
        dircon_ip = None # get from listener after discovery <----- must be a string

        # Setup TCP socket
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

        # Initialise DIRCON connection
        dircon = DIRCON(sock, dircon_ip)
    except KeyboardInterrupt:
        sock.close()
        exit()

if __name__ == '__main__':
    main()