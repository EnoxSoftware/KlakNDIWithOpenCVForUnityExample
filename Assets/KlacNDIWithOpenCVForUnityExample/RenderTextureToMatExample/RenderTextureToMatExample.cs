using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KlacNDIWithOpenCVForUnityExample
{
    /// <summary>
    /// RenderTexture To Mat Example
    /// </summary>
    public class RenderTextureToMatExample : MonoBehaviour
    {
        [Header("Input")]
        /// <summary>
        /// The RawImage for previewing the result.
        /// </summary>
        public RenderTexture inputRenderTexture;

        [Header("Output")]
        /// <summary>
        /// The RawImage for previewing the result.
        /// </summary>
        public RawImage resultPreview;

        [Space(10)]

        /// <summary>
        /// The graphicsBuffer for Utils.matToRenderTexture().
        /// </summary>
        GraphicsBuffer inputGraphicsBuffer;

        /// <summary>
        /// rgbaMat
        /// </summary>
        Mat rgbaMat;

        /// <summary>
        /// The output RenderTexture.
        /// </summary>
        RenderTexture outputRenderTexture;

        /// <summary>
        /// The graphicsBuffer for Utils.matToRenderTexture().
        /// </summary>
        GraphicsBuffer outputGraphicsBuffer;

        /// <summary>
        /// The FPS monitor.
        /// </summary>
        FpsMonitor fpsMonitor;

        // Use this for initialization
        void Start()
        {
            fpsMonitor = GetComponent<FpsMonitor>();

            if (inputRenderTexture != null)
            {
                rgbaMat = new Mat(inputRenderTexture.height, inputRenderTexture.width, CvType.CV_8UC4, (0, 0, 0, 255));

                inputGraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)rgbaMat.total(), (int)rgbaMat.elemSize());

                outputRenderTexture = new RenderTexture(rgbaMat.width(), rgbaMat.height(), 0);
                outputRenderTexture.enableRandomWrite = true;
                outputRenderTexture.Create();

                outputGraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, (int)rgbaMat.total(), (int)rgbaMat.elemSize());


                resultPreview.texture = outputRenderTexture;
                resultPreview.GetComponent<AspectRatioFitter>().aspectRatio = (float)outputRenderTexture.width / outputRenderTexture.height;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (rgbaMat != null)
            {
                Utils.renderTextureToMat(inputRenderTexture, rgbaMat, inputGraphicsBuffer);


                // Add text overlay on the frame
                Imgproc.putText(rgbaMat, "W:" + rgbaMat.width() + " H:" + rgbaMat.height() + " SO:" + Screen.orientation, (5, rgbaMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 1.0, (255, 255, 255, 255), 2, Imgproc.LINE_AA, false);


                Utils.matToRenderTexture(rgbaMat, outputRenderTexture, outputGraphicsBuffer);
            }
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {

            ReleaseResources();
        }

        /// <summary>
        /// To release the resources.
        /// </summary>
        void ReleaseResources()
        {

            if (rgbaMat != null)
            {
                rgbaMat.Dispose();
            }

            if (inputGraphicsBuffer != null)
            {
                inputGraphicsBuffer.Dispose();
                inputGraphicsBuffer = null;
            }

            // Destroy the texture and set it to null
            if (outputRenderTexture != null)
            {
                RenderTexture.Destroy(outputRenderTexture);
                outputRenderTexture = null;
            }

            if (outputGraphicsBuffer != null)
            {
                outputGraphicsBuffer.Dispose();
                outputGraphicsBuffer = null;
            }
        }

        /// <summary>
        /// Raises the back button click event.
        /// </summary>
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("KlacNDIWithOpenCVForUnityExample");
        }
    }
}