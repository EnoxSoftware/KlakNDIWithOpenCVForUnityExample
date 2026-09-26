using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityIntegration;
using OpenCVForUnity.UnityIntegration.Helper.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KlakNDIWithOpenCVForUnityExample
{
    /// <summary>
    /// RenderTexture To Mat Example
    /// </summary>
    public class RenderTextureToMatExample : MonoBehaviour
    {
        // Public Fields
        [Header("Input")]
        /// <summary>
        /// The RawImage for previewing the result.
        /// </summary>
        public RenderTexture InputRenderTexture;

        [Header("Output")]
        /// <summary>
        /// The RawImage for previewing the result.
        /// </summary>
        public RawImage ResultPreview;

        [Space(10)]

        // Private Fields
        /// <summary>
        /// The graphicsBuffer for Utils.matToRenderTexture().
        /// </summary>
        private GraphicsBuffer _inputGraphicsBuffer;

        /// <summary>
        /// rgbaMat
        /// </summary>
        private Mat _rgbaMat;

        /// <summary>
        /// The output RenderTexture.
        /// </summary>
        private RenderTexture _outputRenderTexture;

        /// <summary>
        /// The graphicsBuffer for Utils.matToRenderTexture().
        /// </summary>
        private GraphicsBuffer _outputGraphicsBuffer;

        /// <summary>
        /// The FPS monitor.
        /// </summary>
        private FpsMonitor _fpsMonitor;

        // Unity Lifecycle Methods
        private void Start()
        {
            _fpsMonitor = GetComponent<FpsMonitor>();

            if (InputRenderTexture != null)
            {
                _rgbaMat = new Mat(InputRenderTexture.height, InputRenderTexture.width, CvType.CV_8UC4, (0, 0, 0, 255));

                _inputGraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)_rgbaMat.total(), (int)_rgbaMat.elemSize());

                _outputRenderTexture = new RenderTexture(_rgbaMat.width(), _rgbaMat.height(), 0);
                _outputRenderTexture.enableRandomWrite = true;
                _outputRenderTexture.Create();

                _outputGraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)_rgbaMat.total(), (int)_rgbaMat.elemSize());

                ResultPreview.texture = _outputRenderTexture;
                ResultPreview.GetComponent<AspectRatioFitter>().aspectRatio = (float)_outputRenderTexture.width / _outputRenderTexture.height;
            }
        }

        private void Update()
        {
            if (_rgbaMat != null)
            {
                OpenCVMatUnityUtils.RenderTextureToMat(InputRenderTexture, _rgbaMat, _inputGraphicsBuffer);

                // Add text overlay on the frame
                Imgproc.putText(_rgbaMat, "W:" + _rgbaMat.width() + " H:" + _rgbaMat.height() + " SO:" + Screen.orientation, (5, _rgbaMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 1.0, (255, 255, 255, 255), 2, Imgproc.LINE_AA, false);

                OpenCVMatUnityUtils.MatToRenderTexture(_rgbaMat, _outputRenderTexture, _outputGraphicsBuffer);
            }
        }

        private void OnDestroy()
        {
            ReleaseResources();
        }

        // Public Methods
        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("KlakNDIWithOpenCVForUnityExample");
        }

        // Private Methods
        /// <summary>
        /// To release the resources.
        /// </summary>
        private void ReleaseResources()
        {
            _rgbaMat?.Dispose(); _rgbaMat = null;
            _inputGraphicsBuffer?.Dispose(); _inputGraphicsBuffer = null;

            // Destroy the texture and set it to null
            if (_outputRenderTexture != null)
            {
                RenderTexture.Destroy(_outputRenderTexture);
                _outputRenderTexture = null;
            }

            _outputGraphicsBuffer?.Dispose(); _outputGraphicsBuffer = null;
        }
    }
}
