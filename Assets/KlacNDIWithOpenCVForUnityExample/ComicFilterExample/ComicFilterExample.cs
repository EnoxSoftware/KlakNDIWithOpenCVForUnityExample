using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KlacNDIWithOpenCVForUnityExample
{
    /// <summary>
    /// Comic Filter Example
    /// An example of image processing (comic filter) using the Imgproc class.
    /// Referring to http://dev.classmethod.jp/smartphone/opencv-manga-2/.
    /// </summary>
    [RequireComponent(typeof(MultiSource2MatHelper))]
    public class ComicFilterExample : MonoBehaviour
    {
        [Header("Output")]
        /// <summary>
        /// The RawImage for previewing the result.
        /// </summary>
        public RawImage resultPreview;

        [Space(10)]

        /// <summary>
        /// Whether RenderTexture is used when displaying rgbaMat in the scene; if Off, Texture2D is used.
        /// </summary>
        public Toggle outputRenderTextureToggle;

        /// <summary>
        /// The comic filter.
        /// </summary>
        ComicFilter comicFilter;

        /// <summary>
        /// The texture.
        /// </summary>
        Texture2D outputTexture2D;

        /// <summary>
        /// The output RenderTexture.
        /// </summary>
        RenderTexture outputRenderTexture;

        /// <summary>
        /// The graphicsBuffer for Utils.matToRenderTexture().
        /// </summary>
        GraphicsBuffer graphicsBuffer;

        /// <summary>
        /// The multi source to mat helper.
        /// </summary>
        MultiSource2MatHelper multiSource2MatHelper;

        /// <summary>
        /// The FPS monitor.
        /// </summary>
        FpsMonitor fpsMonitor;

        // Use this for initialization
        void Start()
        {
            fpsMonitor = GetComponent<FpsMonitor>();

            multiSource2MatHelper = gameObject.GetComponent<MultiSource2MatHelper>();
            multiSource2MatHelper.outputColorFormat = Source2MatHelperColorFormat.RGBA;
            multiSource2MatHelper.Initialize();
        }

        /// <summary>
        /// Raises the source to mat helper initialized event.
        /// </summary>
        public void OnSourceToMatHelperInitialized()
        {
            Debug.Log("OnSourceToMatHelperInitialized");

            // Retrieve the current frame from the Source2MatHelper as a Mat object
            Mat rgbaMat = multiSource2MatHelper.GetMat();
            rgbaMat.setTo((0, 0, 0, 255));

            if (!outputRenderTextureToggle.isOn)
            {
                // Create a new Texture2D with the same dimensions as the Mat and RGBA32 color format
                outputTexture2D = new Texture2D(rgbaMat.cols(), rgbaMat.rows(), TextureFormat.RGBA32, false);

                // Convert the Mat to a Texture2D, effectively transferring the image data
                Utils.matToTexture2D(rgbaMat, outputTexture2D);

                // Set the Texture2D as the texture of the RawImage for preview.
                resultPreview.texture = outputTexture2D;
                resultPreview.GetComponent<AspectRatioFitter>().aspectRatio = (float)outputTexture2D.width / outputTexture2D.height;
            }
            else
            {
                graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)rgbaMat.total(), (int)rgbaMat.elemSize());

                outputRenderTexture = new RenderTexture(rgbaMat.width(), rgbaMat.height(), 0);
                outputRenderTexture.enableRandomWrite = true;
                outputRenderTexture.Create();

                try
                {
                    // Convert the Mat to a RenderTexture, effectively transferring the image data
                    Utils.matToRenderTexture(rgbaMat, outputRenderTexture, graphicsBuffer);
                }
                catch (Exception ex)
                {
                    if (fpsMonitor != null)
                        fpsMonitor.consoleText = ex.Message;
                }

                // Set the RenderTexture as the texture of the RawImage for preview.
                resultPreview.texture = outputRenderTexture;
                resultPreview.GetComponent<AspectRatioFitter>().aspectRatio = (float)outputRenderTexture.width / outputRenderTexture.height;

            }

            if (fpsMonitor != null)
            {
                fpsMonitor.Add("width", rgbaMat.width().ToString());
                fpsMonitor.Add("height", rgbaMat.height().ToString());
                fpsMonitor.Add("orientation", Screen.orientation.ToString());
            }

            int thickness = (Mathf.Max(rgbaMat.width(), rgbaMat.height()) <= 640) ? 3 : 5;
            comicFilter = new ComicFilter(60, 120, thickness);
        }

        /// <summary>
        /// Raises the source to mat helper disposed event.
        /// </summary>
        public void OnSourceToMatHelperDisposed()
        {
            Debug.Log("OnSourceToMatHelperDisposed");

            comicFilter.Dispose();

            ReleaseResources();
        }

        /// <summary>
        /// Raises the source to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Message.</param>
        public void OnSourceToMatHelperErrorOccurred(Source2MatHelperErrorCode errorCode, string message)
        {
            Debug.Log("OnSourceToMatHelperErrorOccurred " + errorCode + ":" + message);

            if (fpsMonitor != null)
            {
                fpsMonitor.consoleText = "ErrorCode: " + errorCode + ":" + message;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (multiSource2MatHelper.IsPlaying() && multiSource2MatHelper.DidUpdateThisFrame())
            {
                Mat rgbaMat = multiSource2MatHelper.GetMat();

                comicFilter.Process(rgbaMat, rgbaMat);

                // Add text overlay on the frame
                Imgproc.putText(rgbaMat, "W:" + rgbaMat.width() + " H:" + rgbaMat.height() + " SO:" + Screen.orientation, (5, rgbaMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 1.0, (255, 255, 255, 255), 2, Imgproc.LINE_AA, false);


                if (!outputRenderTextureToggle.isOn)
                {
                    // Convert the Mat to a Texture2D to display it on a texture
                    Utils.matToTexture2D(rgbaMat, outputTexture2D);
                }
                else
                {
                    // Convert the Mat to a RenderTexture to display it on a texture
                    Utils.matToRenderTexture(rgbaMat, outputRenderTexture, graphicsBuffer);
                }
            }
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {
            multiSource2MatHelper.Dispose();
        }

        /// <summary>
        /// To release the resources.
        /// </summary>
        void ReleaseResources()
        {
            // Destroy the texture and set it to null
            if (outputTexture2D != null)
            {
                Texture2D.Destroy(outputTexture2D);
                outputTexture2D = null;
            }

            // Destroy the texture and set it to null
            if (outputRenderTexture != null)
            {
                RenderTexture.Destroy(outputRenderTexture);
                outputRenderTexture = null;
            }

            if (graphicsBuffer != null)
            {
                graphicsBuffer.Dispose();
                graphicsBuffer = null;
            }
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("KlacNDIWithOpenCVForUnityExample");
        }

        /// <summary>
        /// Raises the play button click event.
        /// </summary>
        public void OnPlayButtonClick()
        {
            multiSource2MatHelper.Play();
        }

        /// <summary>
        /// Raises the pause button click event.
        /// </summary>
        public void OnPauseButtonClick()
        {
            multiSource2MatHelper.Pause();
        }

        /// <summary>
        /// Raises the stop button click event.
        /// </summary>
        public void OnStopButtonClick()
        {
            multiSource2MatHelper.Stop();
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCameraButtonClick()
        {
            multiSource2MatHelper.requestedIsFrontFacing = !multiSource2MatHelper.requestedIsFrontFacing;
        }

        /// <summary>
        /// Raises the output RenderTexture toggle value changed event.
        /// </summary>
        public void OnOutputRenderTextureToggleValueChanged()
        {
            if (multiSource2MatHelper.IsInitialized())
            {
                multiSource2MatHelper.Initialize();
            }
        }
    }
}