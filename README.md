---
page_type: sample
name: Extended eye tracking in Unity
description: This sample shows you how to use extended eye tracking features in Unity projects using a HoloLens.
languages:
- csharp
products:
- hololens
---

# Extended eye tracking in Unity 

![License](https://img.shields.io/badge/license-MIT-green.svg)

Supported device  | Supported Unity versions | Built with XR configuration
:---------------: | :----------------------: | :--------------------------: 
HoloLens 2        | Unity 2020 or higher     | Mixed Reality OpenXR Plugin

This sample shows how to use EyeTracking SDK to access extended eye tracking features in Unity projects using a HoloLens. Covered features include 
* Setting eye tracking data framerate
* Getting individual and combined eye gaze vectors

## Contents

File/folder  | Description |
-------------|-------------|
`SampleEyeTracking/Assets` | Unity assets, scenes, prefabs, and scripts. |
`SampleEyeTracking/Packages` | Project manifest and packages list. |
`SampleEyeTracking/ProjectSettings` | Unity asset setting files. |
`SampleEyeTracking/.gitignore` | Define what to ignore at commit time. |

## Required tools

1. Install the [tools for Mixed Reality development](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/install-the-tools)
2. Install the [recommended Unity version](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/choosing-unity-version) 

This repo uses following versions of tools
* Unity 2020.3.40f1
* Mixed Reality OpenXR Plugin 1.5.1
* Visual Studio 2022 17.3.6

## Setup

1. Clone or download this sample repository.
2. Open the `SampleEyeTracking` folder in Unity Hub and launch the project.
3. Open the `Assets/SampleScene` in Unity.

## Run the sample

Extended eye tracking features are not supported by Holographic Remoting at this time, so running this sample in Unity editor won't get any gaze data. The only way to test it is building and deploying to HoloLens.

## Build and deploy the sample

1. Follow [Build and deploy to the HoloLens](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/build-and-deploy-to-hololens) to build Unity project and deploy to HoloLens.
2. Launching this app in HoloLens and granting the gaze permission in the dialog, the gaze framerate will be set to 90FPS and you will see 3 cubes and 1 cylinder following your gaze direction in 1.5 meter away.
    * The green cube represents your left eye gaze.
    * The red cube represents your right eye gaze.
    * The cyan cube represents your combined eye gaze.
    * The blue cylinder also represents your combined eye gaze but its coordinate is set related to the Unity camera GameObject.

## Project explanation

### Used packages

This Unity sample app use following packages. 

Package  | Description 
-------------|-------------
[Mixed Reality OpenXR Plugin](https://github.com/microsoft/OpenXR-Unity-MixedReality-Samples/releases) | to read the pose of eyeGazeTracker under Unity scene coordinate system, then use to convert the gaze data's coordinate
[NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) | a tool to download and import NuGet package
[Microsoft.MixedReality.EyeTracking](https://www.nuget.org/packages/Microsoft.MixedReality.EyeTracking) | NuGet package to provide eye tracking features
[MRTK Graphic Tools](https://github.com/microsoft/MixedReality-GraphicsTools-Unity) | provide the shaders to render holograms

The `Mixed Reality OpenXR Plugin` and `MRTK Graphic Tools` could be imported into Unity by the [MRTK Feature Tool](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool#download). The `NuGetForUnity` could be imported through Unity's custom package. The `Microsoft.MixedReality.EyeTracking` could be imported through the `NuGetForUnity` tab in Unity editor. 

Depending on your use case, you may don't need to use all these packages in your projects.

**Note**: some of packages above are not open-source. 

### Scripts

The script [ExtendedEyeGazeDataProvider](./SampleEyeTracking/Assets/Scripts/ExtendedEyeGazeDataProvider.cs) is the core script to call APIs and provide gaze data to other scripts in Unity. Specially, it does following things
* Maintain the eyeGazeTracker instance
* Set the framerate of eyeGazeTracker
* Read eyeGaze data from eyeGazeTracker
* Read eyeGazeTracker pose from Mixed Reality OpenXR plugin API
* Convert eyeGaze data into Unity's scene coordinate system or Unity's camera GameObject coordinate system

## Other documentations

* Complete API reference for the Microsoft.MixedReality.EyeTracking NuGet package on at the [NuGet Gallery](https://www.nuget.org/packages/Microsoft.MixedReality.EyeTracking)
* For more details about how to setup your Unity project, refer to [Microsoft Docs](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/extended-eye-tracking-unity).
* For more details about how to setup native project, refer to [Microsoft Docs](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/native/extended-eye-tracking-native).

## Contributing

This project welcomes contributions and suggestions. Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.