#!/bin/bash
# auto connect to the bike by resolving the BLE issues automatically via bluetoothctl
cd /home/pi/
echo "Auto connecting to bike via BLE"
EXPECT_SCRIPT="auto-connect.exp"
expect $EXPECT_SCRIPT
echo "Connection established with bike via BLE"
echo "Initialising driver"
python3 cavebike.py
exit 0