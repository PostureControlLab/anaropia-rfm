# Anaropia RFM

Virtual reality experiment framework to investigate the visual contribution to human balance.

## Requirements

* Windows 10 or higher
* Unity 2021.3.16f1
* SteamVR
* [LabJack LJM library](https://labjack.com/support/software/installers/ljm) (necessary to interface LabJack T4 or T7 DAQ devices)

## Setup

1. Run `setup-repo.bat`. This will create the build directory and necessary filesystem links.
2. Set controllers to never turn off in SteamVR settings.
3. Assign the following tracker roles:
    - Shoulder tracker: Chest role
    - Hip tracker: Waist role
4. Run through Unity editor or build (recommended for better performance).

## Scenes

- `LivingRoom` features a medium-size living room. ![](img/room.png)
- `Lab` is a minimal laboratory environment, where players stand on top of a platform surrounded by darkness. This scene features the scaling screen. ![](img/lab.png)

## Stimuli

Stimuli sets must be placed in `stimuli` directory where also the executable is located . For each set, create a folder with its name. Each set/folder can contain up to 9 stimuli files which correspond to the alphanumeric or numpad keys 1 to 9. The files must be called `vr_stim_<NUMBER>.csv` (e.g. `vr_stim_4.csv`). 
The stimuli set can be selected at runtime for the room (numpad) and screen (number row). The 0 key stops the current running stimuli.

Stimuli are CSV files and must start with a header `time,pitch,roll,yaw,trans_ap,trans_ml,trans_ud,scale`.
Each row contains the absolute time (in seconds), 3D rotation angles (in degree, relative to rotation center), 3D position (in meters, relative to world space position of screen or room) and local scale for all axes. You can specify any combination of columns and except `time`, all columns can be omitted.
The rotation center is exactly 8.8 cm above the center of the floor markers for the screen or room.

The experiment data is written to the same directory from which the `.exe` was started in a folder called `vr_expt_data`. Output CSV files contain:
- Timestamp in seconds
- Pitch, roll, yaw of the stimulus (relative to rotation center)
- Anterior posterior, mediolateral and up-down translation of the stimulus (relative to world space position of screen or room)
- Position and rotation of the HMD (relative to world space origin)
- Position and rotation of the shoulder tracker (relative to world space origin)
- Position and rotation of the hip tracker (relative to world space origin)

All coordinates are in world space. Units are meters and degrees. The coordinate system is left-handed (Unity).
The positive X axis points towards the right (wall with couch and door), the positive Y axis points upwards (ceiling), the positive Z axis points forward (balance screen).

### Room dimensions

Height: 3.172 m

Width (from TV wall to door wall): 5.95 m

Depth (from window wall to bookshelf wall): 7.95 m

Room view position to window wall (along floor): 3.468 m

Room view position to door wall (along floor): 2.175 m

## Screen textures

You can place different textures for the screens in the `Screen Textures` directory and select them at runtime. They must be `.png` or `.jpg` files.

### Screen dimensions

![](img/screen_round.png)
![](img/screen_rect.png)

All the following dimensions are parallel to the floor/walls:

Screen distance from room marker: 1.088 m

Screen distance from screen marker: 0.188 m

Screen distance from floor: 0.33 m

## LabJack

To use analog signals to control the screen/room, attach a LabJack T4 or T7 via USB and set the stimuli to `__LabJack`. In this mode, only manual recording using the `R` key (screen) or `*` (numpad, room) is available. You can freely map the analog input to rotation and translation in the bottom right corner of the UI.
The input `AIN0` of the LabJack T4 or T7 can be used as a trigger to start and stop a recording. As long as the voltage is higher or equal to 1.5V the recording will run.

| Mapping                | Scaling |
|------------------------|---------|
| Screen pitch           | 1°/V    |
| Screen roll            | 1°/V    |
| Screen yaw             | 1°/V    |
| Screen anterior-posterior | 10 cm/V |
| Screen mediolateral    | 10 cm/V |
| Screen up/down         | 10 cm/V |
| Screen scale           | 1:1     |
| Room pitch             | 1°/V    |
| Room roll              | 1°/V    |
| Room yaw               | 1°/V    |
| Room anterior-posterior   | 10 cm/V |
| Room mediolateral      | 10 cm/V |
| Room up/down           | 10 cm/V |
| Room scale             | 1:1     |

## Floor fix

SteamVR tends to offset the floor every now and then, especially when restarting SteamVR/PC or when moving the base stations.

Just make sure to run the room setup every time you restart the PC or tinker with the base stations. Alternatively, OpenVR Advanced Settings provides a floor fixing tool. Install from here:
https://github.com/OpenVR-Advanced-Settings/OpenVR-AdvancedSettings

## Debug console
Any log, warning or error messages can be viewed in-game by clicking the symbol on the right.

![](img/debug_console.png)

It shows the current number of unread messages.