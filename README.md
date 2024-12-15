# KlacNDI With OpenCVForUnity Example
![KlacNDIWithOpenCVForUnityExample](https://user-images.githubusercontent.com/7920392/221954204-416c4240-fb9b-4346-a36a-9f0951666ca4.gif)

## Overview
- Integrate "KlacNDI" with "OpenCV for Unity".
- This is an example of using klakNDI to receive a video stream delivered by the NDI protocol, convert it to the OpenCV Mat class and apply image processing.


## Environment
- Windows / macOS / Linux / Android / iOS
- Unity >= 2022.3.54f1+
- Scripting backend MONO / IL2CPP
- [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?aid=1011l4ehR) 2.6.5+
- [KlacNDI](https://github.com/keijiro/KlakNDI)


## Setup
1. Download the latest release unitypackage. [KlacNDIWithOpenCVForUnityExample.unitypackage](https://github.com/EnoxSoftware/KlacNDIWithOpenCVForUnityExample/releases)
1. Create a new project. (KlacNDIWithOpenCVForUnityExample)
1. Import and Setup [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?aid=1011l4ehR).
    * Download Dnn model files by ExampleAssetsDownloader.
    ![download_dnn_models.png](download_dnn_models.png)
    * Move the files from the "OpenCVForUnity/StreamingAssets/" folder to the "Assets/StreamingAssets" folder.
    ![move_streamingassetsfolder.png](move_streamingassetsfolder.png)
1. Import and Setup [KlacNDI](https://github.com/keijiro/KlakNDI).
1. Import [KlacNDIWithOpenCVForUnityExample.unitypackage](https://github.com/EnoxSoftware/KlacNDIWithOpenCVForUnityExample/releases).
1. Add the "Assets/KlacNDIWithOpenCVForUnityExample/*.unity" files to the "Scenes In Build" list in the "Build Settings" window.
1. Build and Deploy.
    ![setup.png](setup.png)

## ScreenShot
![screenshot01.png](screenshot01.png)
![screenshot02.png](screenshot02.png)
![screenshot03.png](screenshot03.png)
![screenshot04.png](screenshot04.png)

