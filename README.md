# Cycle Cave

a virtual reality (VR) room-scale cave project that integrates a cave system with an IoT bike.

![Cycle cave set up image](Assets/ZqApRFHg.jpg)

## System Overview

* **The Cave system** utilises five projectors covering the front, left, right, floor, and roof. Each projector is connected to an individual PC running the Cycle Cave application.

* The **IoT bike** includes a braking system but no manual turning; turns are generated automatically within the virtual environment. Users follow a pre-defined route as they cycle. The bike is connected to the main PC via Bluetooth. 

* Non-playable object movement is triggered with position radius and trigger collider objects
  
## Getting Started
* **Internet connection required**: The IoT bike transmits movement data to the application, which calculates real-time bike movement in the virtual environment. Other PCs running the application receive this data and synchronize movement accordingly.
* **Data Collection**: The system collects internal data, including user ID, speed, cadence, and time. Data is stored in the same folder as the executable file per session, only the PC being connected to the bike will have speed data stored within their data files. Naming convention follows the following format: bikeDataLog_(userID)_(YearMonthDate)_(HourMinuteSeconds). 
* This repository uses GitLFS to store big data
  
### Scenes
There are five scenes—one practice scene and four randomized cycling environments. Each scene lasts 2 minutes, with a 30-second break in-between denoted with a blank black screen.
* Practice Scene: A basic environment allowing users to familiarize themselves with the IoT bike system.
* Low-Traffic Scenes (2): Open environments with no obstacles for free cycling.
* High-Traffic Scenes (2): Realistic urban settings featuring pedestrian crossings, lane changes, and vehicles entering/exiting roads.

### Dependencies

* Unity 2022.3.27f
* Windows system
* Photon PUN 2 2.47
* TextMeshPro 3.0.7

### Build Settings
* On Player Settings>PC> Ensure Resolution - Fullscreen Mode is set to Fullscreen Window
* Build settings > Scenes In Build, from 0 to 5: Menu, city block, Suburban1, Suburban2, City1, City2
* Target Platform: Windows

### Executing program

* Start the program on the first PC, enter the Session ID number > Create Session > Choose the side of the projection screen that the PC is connected to
* Start the program on the other PC one by one, enter the same Session ID number > Join Session >  Choose the side of the projection screen that the PC is connected to
* After all 5 PC are connected the program will start

## Common issues and advise

* The Camera Count parameter in the Manager script determines how many cameras are required for the program to run. Currently, it is set to 5. Reducing this value allows the program to run with a different number of PCs/screens.
* Each projection screen has its dedicated camera object.
* An error may occur at the start of some scenes when the program attempts to read the bicycle position value. A workaround is to ensure that the main host PC folder contains fewer than three data files. To prevent data loss, it is recommended to continuously back up saved data in another location between sessions. 
  
## Authors
* Farah Farizi - https://github.com/farahfarizi24 / farahdfarizi@gmail.com
* Sanjay Selvaraj - sanjayspostbox@outlook.com
* Ethan Byrne-Staunton- https://github.com/KasparByrne
* Delia Chia Hui Kok - https://github.com/D3lK1ch1

