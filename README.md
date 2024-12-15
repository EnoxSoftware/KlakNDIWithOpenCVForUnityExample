# KlakNDI With OpenCVForUnity Example
![KlakNDIWithOpenCVForUnityExample_1](https://github.com/user-attachments/assets/6fb7432d-e2e9-4c86-aecd-c263dbdffeea)
![KlakNDIWithOpenCVForUnityExample_2](https://github.com/user-attachments/assets/238e9f55-d274-4e61-a7a0-cd30b31a1021)

YouTube Live --> [OBS Studio](https://obsproject.com/) + [DistroAV](https://github.com/DistroAV/DistroAV) --> [KlakNDI](https://github.com/keijiro/KlakNDI) --> [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?aid=1011l4ehR)


## Overview
- Integrate "[KlakNDI](https://github.com/keijiro/KlakNDI)" with "[OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?aid=1011l4ehR)".
- This is an example of using [KlakNDI](https://github.com/keijiro/KlakNDI) to receive a video stream delivered by [NDI](https://ndi.video/)®, convert it to the OpenCV Mat class and apply image processing.


## Environment
- Windows / macOS / Linux / Android / iOS
- Unity >= 2022.3.54f1+
- Scripting backend MONO / IL2CPP
- [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?aid=1011l4ehR) 2.6.5+
- [KlakNDI](https://github.com/keijiro/KlakNDI)


## Setup
1. Download the latest release unitypackage. [KlakNDIWithOpenCVForUnityExample.unitypackage](https://github.com/EnoxSoftware/KlakNDIWithOpenCVForUnityExample/releases)
1. Create a new project. (KlakNDIWithOpenCVForUnityExample)
1. Import and Setup [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?aid=1011l4ehR).
    * Download Dnn model files by ExampleAssetsDownloader.     
      ![download_dnn_models.png](download_dnn_models.png)     
    * Move the files from the "OpenCVForUnity/StreamingAssets/" folder to the "Assets/StreamingAssets" folder.      
      ![move_streamingassetsfolder.png](move_streamingassetsfolder.png)      
1. Import and Setup [KlakNDI](https://github.com/keijiro/KlakNDI).
1. Import [KlakNDIWithOpenCVForUnityExample.unitypackage](https://github.com/EnoxSoftware/KlakNDIWithOpenCVForUnityExample/releases).
1. Add the "Assets/KlakNDIWithOpenCVForUnityExample/*.unity" files to the "Scenes In Build" list in the "Build Settings" window.
1. Build and Deploy.   
   ![setup.png](setup.png)


## ScreenShot
![screenshot01.png](screenshot01.png)
![screenshot02.png](screenshot02.png)
![screenshot03.png](screenshot03.png)
![screenshot04.png](screenshot04.png)

