# VR-Eyetracking-Framework
A framework for gathering, processing, classifying and visualizing gaze data in Unity

## Prerequesites
* Unity
* The OpenXR Plugin for Unity
* A HMD that supports Eyetracking via OpenXR

## Usage
To use the framework, add the `GazeDataRecorder` and `GazeDataDrawer` (optional) Scripts to Unity GameObjects as components.
You have to set the corresponding OpenXR Input Actions in the Inspector.
You can also customize how the data is being processed in the Inspector.
The data is saved as a .csv file to your applications `PersistentDataPath`. 

### Important
In order to not always collect data it is recommended to implement some sort of logic that enables or disables the GameObjects, as that is when the corresponding functions are called.

Note: `GazeDataDrawer` is only needed if you want to visualize the data in the scene
