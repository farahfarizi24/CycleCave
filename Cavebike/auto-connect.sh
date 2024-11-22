#!/bin/bash
# auto connect to the bike by resolving the BLE issues automatically via bluetoothctl
echo "Auto connecting to bike via BLE"
EXPECT_SCRIPT="auto-connect.exp"
expect $EXPECT_SCRIPT
echo "Connection established with bike via BLE"
exit 0