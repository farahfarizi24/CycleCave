# Windows Powershell Launch Script
# Script Generated On Thursday, 31 October 2024, 10:43:34 AM
# Setup contains 5 displays and 0 display managers

# Display: Front Wall
If ($env:ComputerName -eq '01-DK-FRONT') {
	& '.\UniCAVE2018.exe' -screen-fullscreen 0 -popupwindow -vrmode stereo
}

# Display: Left Wall
If ($env:ComputerName -eq '03-DK-LEFT') {
	& '.\UniCAVE2018.exe' -screen-fullscreen 0 -popupwindow -vrmode stereo
}

# Display: Right
If ($env:ComputerName -eq '04-DK-RIGHT') {
	& '.\UniCAVE2018.exe' -screen-fullscreen 0 -popupwindow -vrmode stereo
}

# Display: FloorLeft
If ($env:ComputerName -eq '05-DK-FLOOR') {
	& '.\UniCAVE2018.exe' -screen-fullscreen 0 -popupwindow -vrmode stereo
}

# Display: Roof
If ($env:ComputerName -eq '02-DK-ROOF') {
	& '.\UniCAVE2018.exe' -screen-fullscreen 0 -popupwindow -vrmode stereo
}