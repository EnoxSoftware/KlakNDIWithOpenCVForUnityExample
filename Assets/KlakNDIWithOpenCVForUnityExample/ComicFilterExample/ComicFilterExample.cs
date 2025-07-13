using System;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityIntegration;
using OpenCVForUnity.UnityIntegration.Helper.Source2Mat;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KlakNDIWithOpenCVForUnityExample
{
    /// <summary>
    /// Comic Filter Example
    /// An example of image processing (comic filter) using the Imgproc class.
    /// Referring to http://dev.classmethod.jp/smartphone/opencv-manga-2/.
    /// </summary>
    [RequireComponent(typeof(MultiSource2MatHelper))]
    public class ComicFilterExample : MonoBehaviour
    {
        // Public Fields
        [Header("Output")]
        /// <summary>
        /// The RawImage for previewing the result.
        /// </summary>
        public RawImage ResultPreview;

        [Space(10)]

        /// <summary>
        /// Whether RenderTexture is used when displaying rgbaMat in the scene; if Off, Texture2D is used.
        /// </summary>
        public Toggle OutputRenderTextureToggle;

        // Private Fields
        /// <summary>
        /// The comic filter.
        /// </summary>
        private ComicFilter _comicFilter;

        /// <summary>
        /// The texture.
        /// </summary>
        private Texture2D _outputTexture2D;

        /// <summary>
        /// The output RenderTexture.
        /// </summary>
        private RenderTexture _outputRenderTexture;

        /// <summary>
        /// The graphicsBuffer for Utils.matToRenderTexture().
        /// </summary>
        private GraphicsBuffer _graphicsBuffer;

        /// <summary>
        /// The multi source to mat helper.
        /// </summary>
        private MultiSource2MatHelper _multiSource2MatHelper;

        /// <summary>
        /// The FPS monitor.
        /// </summary>
        private FpsMonitor _fpsMonitor;

        // Unity Lifecycle Methods
        private void Start()
        {
            _fpsMonitor = GetComponent<FpsMonitor>();

            _multiSource2MatHelper = gameObject.GetComponent<MultiSource2MatHelper>();
            _multiSource2MatHelper.OutputColorFormat = Source2MatHelperColorFormat.RGBA;
            _multiSource2MatHelper.Initialize();
        }

        private void Update()
        {
            if (_multiSource2MatHelper.IsPlaying() && _multiSource2MatHelper.DidUpdateThisFrame())
            {
                Mat rgbaMat = _multiSource2MatHelper.GetMat();

                _comicFilter.Process(rgbaMat, rgbaMat);

                // Add text overlay on the frame
                Imgproc.putText(rgbaMat, "W:" + rgbaMat.width() + " H:" + rgbaMat.height() + " SO:" + Screen.orientation, (5, rgbaMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 1.0, (255, 255, 255, 255), 2, Imgproc.LINE_AA, false);

                if (!OutputRenderTextureToggle.isOn)
                {
                    // Convert the Mat to a Texture2D to display it on a texture
                    OpenCVMatUtils.MatToTexture2D(rgbaMat, _outputTexture2D);
                }
                else
                {
                    // Convert the Mat to a RenderTexture to display it on a texture
                    OpenCVMatUtils.MatToRenderTexture(rgbaMat, _outputRenderTexture, _graphicsBuffer);
                }
            }
        }

        private void OnDestroy()
        {
            _multiSource2MatHelper?.Dispose();
        }

        // Public Methods
        /// <summary>
        /// Raises the source to mat helper initialized event.
        /// </summary>
        public void OnSourceToMatHelperInitialized()
        {
            Debug.Log("OnSourceToMatHelperInitialized");

            // Retrieve the current frame from the Source2MatHelper as a Mat object
            Mat rgbaMat = _multiSource2MatHelper.GetMat();
            rgbaMat.setTo((0, 0, 0, 255));

            if (!OutputRenderTextureToggle.isOn)
            {
                // Create a new Texture2D with the same dimensions as the Mat and RGBA32 color format
                _outputTexture2D = new Texture2D(rgbaMat.cols(), rgbaMat.rows(), TextureFormat.RGBA32, false);

                // Convert the Mat to a Texture2D, effectively transferring the image data
                OpenCVMatUtils.MatToTexture2D(rgbaMat, _outputTexture2D);

                // Set the Texture2D as the texture of the RawImage for preview.
                ResultPreview.texture = _outputTexture2D;
                ResultPreview.GetComponent<AspectRatioFitter>().aspectRatio = (float)_outputTexture2D.width / _outputTexture2D.height;
            }
            else
            {
                _graphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)rgbaMat.total(), (int)rgbaMat.elemSize());

                _outputRenderTexture = new RenderTexture(rgbaMat.width(), rgbaMat.height(), 0);
                _outputRenderTexture.enableRandomWrite = true;
                _outputRenderTexture.Create();

                try
                {
                    // Convert the Mat to a RenderTexture, effectively transferring the image data
                    OpenCVMatUtils.MatToRenderTexture(rgbaMat, _outputRenderTexture, _graphicsBuffer);
                }
                catch (Exception ex)
                {
                    if (_fpsMonitor != null)
                        _fpsMonitor.ConsoleText = ex.Message;
                }

                // Set the RenderTexture as the texture of the RawImage for preview.
                ResultPreview.texture = _outputRenderTexture;
                ResultPreview.GetComponent<AspectRatioFitter>().aspectRatio = (float)_outputRenderTexture.width / _outputRenderTexture.height;
            }

            if (_fpsMonitor != null)
            {
                _fpsMonitor.Add("width", rgbaMat.width().ToString());
                _fpsMonitor.Add("height", rgbaMat.height().ToString());
                _fpsMonitor.Add("orientation", Screen.orientation.ToString());
            }

            int thickness = (Mathf.Max(rgbaMat.width(), rgbaMat.height()) <= 640) ? 3 : 5;
            _comicFilter = new ComicFilter(60, 120, thickness);
        }

        /// <summary>
        /// Raises the source to mat helper disposed event.
        /// </summary>
        public void OnSourceToMatHelperDisposed()
        {
            Debug.Log("OnSourceToMatHelperDisposed");

            _comicFilter?.Dispose();

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

            if (_fpsMonitor != null)
            {
                _fpsMonitor.ConsoleText = "ErrorCode: " + errorCode + ":" + message;
            }
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("KlakNDIWithOpenCVForUnityExample");
        }

        /// <summary>
        /// Raises the play button click event.
        /// </summary>
        public void OnPlayButtonClick()
        {
            _multiSource2MatHelper.Play();
        }

        /// <summary>
        /// Raises the pause button click event.
        /// </summary>
        public void OnPauseButtonClick()
        {
            _multiSource2MatHelper.Pause();
        }

        /// <summary>
        /// Raises the stop button click event.
        /// </summary>
        public void OnStopButtonClick()
        {
            _multiSource2MatHelper.Stop();
        }

        /// <summary>
        /// Raises the change camera button click event.
        /// </summary>
        public void OnChangeCameraButtonClick()
        {
            _multiSource2MatHelper.RequestedIsFrontFacing = !_multiSource2MatHelper.RequestedIsFrontFacing;
        }

        /// <summary>
        /// Raises the output RenderTexture toggle value changed event.
        /// </summary>
        public void OnOutputRenderTextureToggleValueChanged()
        {
            if (_multiSource2MatHelper.IsInitialized())
            {
                _multiSource2MatHelper.Initialize();
            }
        }

        // Private Methods
        /// <summary>
        /// To release the resources.
        /// </summary>
        private void ReleaseResources()
        {
            // Destroy the texture and set it to null
            if (_outputTexture2D != null)
            {
                Texture2D.Destroy(_outputTexture2D);
                _outputTexture2D = null;
            }

            // Destroy the texture and set it to null
            if (_outputRenderTexture != null)
            {
                RenderTexture.Destroy(_outputRenderTexture);
                _outputRenderTexture = null;
            }

            _graphicsBuffer?.Dispose(); _graphicsBuffer = null;
        }
    }
}
