import socket
import json
import time
import random

# Unity server IP and port (update if needed)
UNITY_IP = "127.0.0.1"  # Match this with Unity script's serverIP
UNITY_PORT = 5005       # Match this with Unity script's serverPort

def generate_dummy_data():
    """Generates dummy data for the bike."""
    data = {
        "speed": round(random.uniform(3.0, 10.0), 2),  # Speed in km/h
        "cadence": round(random.uniform(60.0, 120.0), 2),  # Cadence in RPM
        "power": random.randint(50, 300)  # Power in watts
    }
    return data

def main():
    # Create a UDP socket
    udp_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    print(f"Sending data to Unity at {UNITY_IP}:{UNITY_PORT}")

    try:
        while True:
            # Generate dummy data
            dummy_data = generate_dummy_data()
            # Serialize data to JSON
            json_data = json.dumps(dummy_data)
            print(f"Sending: {json_data}")
            # Send data to Unity
            udp_socket.sendto(json_data.encode('utf-8'), (UNITY_IP, UNITY_PORT))
            # Wait for a short period before sending the next data point
            time.sleep(1)
    except KeyboardInterrupt:
        print("\nStopping the dummy data generator.")
    finally:
        udp_socket.close()

if __name__ == "__main__":
    main()
