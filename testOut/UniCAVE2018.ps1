# Windows Powershell Launch Script
# Script Generated On October 1, 2019, 2:28:50 PM
# Setup contains 4 displays and 1 display managers

# Display Group: Displays
If ($env:ComputerName -eq 'OLIPC') {
	& '.\UniCAVE2018.exe' -screen-fullscreen 1 -adapter 0 -screen-width 5120 -screen-height 720 -vrmode stereo
}