# KlakNDI With OpenCVForUnity Example

![KlakNDIWithOpenCVForUnityExample_1](https://github.com/user-attachments/assets/6fb7432d-e2e9-4c86-aecd-c263dbdffeea)
![KlakNDIWithOpenCVForUnityExample_2](https://github.com/user-attachments/assets/238e9f55-d274-4e61-a7a0-cd30b31a1021)

YouTube Live --> [OBS Studio](https://obsproject.com/) + [DistroAV](https://github.com/DistroAV/DistroAV) --> [KlakNDI](https://github.com/keijiro/KlakNDI) --> [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?aid=1011l4ehR)

## Overview

- Integrate **[KlakNDI](https://github.com/keijiro/KlakNDI)** with **[OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?aid=1011l4ehR)**.
- This is an example of using [KlakNDI](https://github.com/keijiro/KlakNDI) to receive a video stream delivered by **[NDI](https://ndi.video/)**®, convert it to **OpenCV**'s `Mat` class, and apply image processing.

## Environment

- **Windows** / **macOS** / **Linux** / **Android** / **iOS**
- **Unity 2022.3.62f3+**
- **Scripting Backend**: **Mono** / **IL2CPP**
- [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?aid=1011l4ehR) **3.0.4+**
- [KlakNDI](https://github.com/keijiro/KlakNDI)

## Setup

1. Download the latest release unitypackage from [KlakNDIWithOpenCVForUnityExample.unitypackage](https://github.com/EnoxSoftware/KlakNDIWithOpenCVForUnityExample/releases).
2. Create a new project. *(ex. KlakNDIWithOpenCVForUnityExample)*
3. Import and Setup [OpenCV for Unity](https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088?aid=1011l4ehR).
    - Download **Dnn** model files by `Example Assets Downloader` window.  
      ![download_dnn_models.png](images/download_dnn_models.png)
    - Move the files from the `Assets/OpenCVForUnity/StreamingAssets` folder to the `Assets/StreamingAssets` folder.  
      ![move_streamingassetsfolder.png](images/move_streamingassetsfolder.png)
4. Import and Setup [KlakNDI](https://github.com/keijiro/KlakNDI).
5. Import [KlakNDIWithOpenCVForUnityExample.unitypackage](https://github.com/EnoxSoftware/KlakNDIWithOpenCVForUnityExample/releases).
6. Add all of the `***.unity` files in the `KlakNDIWithOpenCVForUnityExample` folder to `Build Settings` → `Scenes In Build`.
7. Build and Deploy.  
    ![setup.png](images/setup.png)

## ScreenShot

![screenshot01.png](images/screenshot01.png)
![screenshot02.png](images/screenshot02.png)
![screenshot03.png](images/screenshot03.png)
![screenshot04.png](images/screenshot04.png)
