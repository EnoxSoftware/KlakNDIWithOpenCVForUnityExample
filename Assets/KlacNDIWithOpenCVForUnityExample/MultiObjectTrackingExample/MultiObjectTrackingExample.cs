using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnity.UnityUtils.MOT.ByteTrack;
#if !UNITY_WSA_10_0
using OpenCVForUnityExample.DnnModel;
#endif
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace KlacNDIWithOpenCVForUnityExample
{
    /// <summary>
    /// Multi Object Tracking (MOT) Example
    /// An example of tracking object detection results using the MOT (Multi Object Tracking) algorithm.
    /// 
    /// ByteTrack: https://github.com/ifzhang/ByteTrack
    /// </summary>
    [RequireComponent(typeof(MultiSource2MatHelper))]
    public class MultiObjectTrackingExample : MonoBehaviour
    {
        [Header("Output")]
        /// <summary>
        /// The RawImage for previewing the result.
        /// </summary>
        public RawImage resultPreview;

        [Space(10)]

        public Toggle showObjectDetectorResultToggle;

        public bool showObjectDetectorResult;

        public Toggle trackerByteTrackToggle;

        public bool trackerByteTrack;

        BYTETracker byteTracker;

        List<Scalar> palette;

#if !UNITY_WSA_10_0
        YOLOXObjectDetector objectDetector;
#endif

        bool disableObjectDetector = false;

        [TooltipAttribute("Path to a binary file of model contains trained weights. It could be a file with extensions .caffemodel (Caffe), .pb (TensorFlow), .t7 or .net (Torch), .weights (Darknet).")]
        public string model = "yolox_tiny.onnx";

        [TooltipAttribute("Path to a text file of model contains network configuration. It could be a file with extensions .prototxt (Caffe), .pbtxt (TensorFlow), .cfg (Darknet).")]
        public string config = "";

        [TooltipAttribute("Optional path to a text file with names of classes to label detected objects.")]
        public string classes = "coco.names";

        [TooltipAttribute("Confidence threshold.")]
        public float confThreshold = 0.25f;

        [TooltipAttribute("Non-maximum suppression threshold.")]
        public float nmsThreshold = 0.45f;

        [TooltipAttribute("Maximum detections per image.")]
        public int topK = 1000;

        [TooltipAttribute("Preprocess input image by resizing to a specific width.")]
        public int inpWidth = 416;

        [TooltipAttribute("Preprocess input image by resizing to a specific height.")]
        public int inpHeight = 416;

        protected string classes_filepath;
        protected string config_filepath;
        protected string model_filepath;

        [Space(10)]

        /// <summary>
        /// Whether RenderTexture is used when displaying rgbaMat in the scene; if Off, Texture2D is used.
        /// </summary>
        public Toggle outputRenderTextureToggle;

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
        /// The bgr mat.
        /// </summary>
        Mat bgrMat;

        /// <summary>
        /// The FPS monitor.
        /// </summary>
        FpsMonitor fpsMonitor;

        /// <summary>
        /// VIDEO_FILENAME
        /// </summary>
        protected static readonly string VIDEO_FILENAME = "OpenCVForUnity/768x576_mjpeg.mjpeg";

        /// <summary>
        /// The CancellationTokenSource.
        /// </summary>
        CancellationTokenSource cts = new CancellationTokenSource();

        // Use this for initialization
        async void Start()
        {
            fpsMonitor = GetComponent<FpsMonitor>();

            multiSource2MatHelper = gameObject.GetComponent<MultiSource2MatHelper>();

            // Update GUI state
            trackerByteTrackToggle.isOn = trackerByteTrack;
            showObjectDetectorResultToggle.isOn = showObjectDetectorResult;

            // Asynchronously retrieves the readable file path from the StreamingAssets directory.
            if (fpsMonitor != null)
                fpsMonitor.consoleText = "Preparing file access...";

            if (!string.IsNullOrEmpty(classes))
            {
                classes_filepath = await Utils.getFilePathAsyncTask("OpenCVForUnity/dnn/" + classes, cancellationToken: cts.Token);
                if (string.IsNullOrEmpty(classes_filepath)) Debug.Log("The file:" + classes + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }
            if (!string.IsNullOrEmpty(config))
            {
                config_filepath = await Utils.getFilePathAsyncTask("OpenCVForUnity/dnn/" + config, cancellationToken: cts.Token);
                if (string.IsNullOrEmpty(config_filepath)) Debug.Log("The file:" + config + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }
            if (!string.IsNullOrEmpty(model))
            {
                model_filepath = await Utils.getFilePathAsyncTask("OpenCVForUnity/dnn/" + model, cancellationToken: cts.Token);
                if (string.IsNullOrEmpty(model_filepath)) Debug.Log("The file:" + model + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }

            if (fpsMonitor != null)
                fpsMonitor.consoleText = "";

            CheckFilePaths();
            Run();
        }

        void CheckFilePaths()
        {
            if (string.IsNullOrEmpty(model_filepath))
            {
                showObjectDetectorResultToggle.isOn = showObjectDetectorResultToggle.interactable = false;
                disableObjectDetector = true;
            }
        }

        void Run()
        {
            if (string.IsNullOrEmpty(model_filepath) || string.IsNullOrEmpty(classes_filepath))
            {
                Debug.LogError("model: " + model + " or " + "config: " + config + " or " + "classes: " + classes + " is not loaded. Please read “StreamingAssets/OpenCVForUnity/dnn/setup_dnn_module.pdf” to make the necessary setup.");
            }
            else
            {
#if !UNITY_WSA_10_0
                objectDetector = new YOLOXObjectDetector(model_filepath, config_filepath, classes_filepath, new Size(inpWidth, inpHeight), confThreshold, nmsThreshold, topK);
#endif
            }

            if (string.IsNullOrEmpty(multiSource2MatHelper.requestedVideoFilePath))
                multiSource2MatHelper.requestedVideoFilePath = VIDEO_FILENAME;
            //multiSource2MatHelper.outputColorFormat = Source2MatHelperColorFormat.RGB; // Tracking API must handle 3 channels Mat image.
            multiSource2MatHelper.Initialize();

            palette = new List<Scalar>();
            palette.Add(new Scalar(255, 56, 56, 255));
            palette.Add(new Scalar(255, 157, 151, 255));
            palette.Add(new Scalar(255, 112, 31, 255));
            palette.Add(new Scalar(255, 178, 29, 255));
            palette.Add(new Scalar(207, 210, 49, 255));
            palette.Add(new Scalar(72, 249, 10, 255));
            palette.Add(new Scalar(146, 204, 23, 255));
            palette.Add(new Scalar(61, 219, 134, 255));
            palette.Add(new Scalar(26, 147, 52, 255));
            palette.Add(new Scalar(0, 212, 187, 255));
            palette.Add(new Scalar(44, 153, 168, 255));
            palette.Add(new Scalar(0, 194, 255, 255));
            palette.Add(new Scalar(52, 69, 147, 255));
            palette.Add(new Scalar(100, 115, 255, 255));
            palette.Add(new Scalar(0, 24, 236, 255));
            palette.Add(new Scalar(132, 56, 255, 255));
            palette.Add(new Scalar(82, 0, 133, 255));
            palette.Add(new Scalar(203, 56, 255, 255));
            palette.Add(new Scalar(255, 149, 200, 255));
            palette.Add(new Scalar(255, 55, 199, 255));
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
            Debug.Log("rgbaMat" + rgbaMat.ToString());

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

            int fps = 30;
            if (multiSource2MatHelper.source2MatHelper is ICameraSource2MatHelper cameraHelper)
            {
                fps = (int)cameraHelper.GetFPS();
            }
            else if (multiSource2MatHelper.source2MatHelper is IVideoSource2MatHelper videoHelper)
            {
                fps = (int)videoHelper.GetFPS();
            }

            byteTracker = new BYTETracker(fps, 30);
            //Debug.Log("fps: " + fps);

            bgrMat = new Mat(rgbaMat.rows(), rgbaMat.cols(), CvType.CV_8UC3);
        }

        /// <summary>
        /// Raises the source to mat helper disposed event.
        /// </summary>
        public void OnSourceToMatHelperDisposed()
        {
            Debug.Log("OnSourceToMatHelperDisposed");

            ResetTrackers();

            if (bgrMat != null)
                bgrMat.Dispose();

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
            if (!multiSource2MatHelper.IsInitialized())
                return;

            if (!multiSource2MatHelper.IsPlaying())
                multiSource2MatHelper.Play();

            if (multiSource2MatHelper.IsPlaying() && multiSource2MatHelper.DidUpdateThisFrame())
            {
                Mat rgbaMat = multiSource2MatHelper.GetMat();

#if UNITY_WSA_10_0
                Imgproc.putText(rgbMat, "Disable the DNN module-dependent Tracker on UWP platforms.", new Point(5, rgbMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
#else
                if (objectDetector == null)
                {
                    Imgproc.putText(rgbaMat, "model file is not loaded.", new Point(5, rgbaMat.rows() - 30), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                    Imgproc.putText(rgbaMat, "Please read console message.", new Point(5, rgbaMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                }
                else
                {
                    if (!disableObjectDetector)
                    {
                        Imgproc.cvtColor(rgbaMat, bgrMat, Imgproc.COLOR_RGBA2BGR);

                        //TickMeter tm = new TickMeter();
                        //tm.start();

                        Mat results = objectDetector.infer(bgrMat);

                        //tm.stop();
                        //Debug.Log("ObjectDetector Inference time (preprocess + infer + postprocess), ms: " + tm.getTimeMilli());

                        Imgproc.cvtColor(bgrMat, rgbaMat, Imgproc.COLOR_BGR2RGBA);

                        if (showObjectDetectorResultToggle.isOn)
                            objectDetector.visualize(rgbaMat, results, false, true);

                        if (trackerByteTrackToggle.isOn)
                        {
                            // update trackers.
                            List<Detection> inputs = ConvertToByteTrackDetections(results);
                            List<Track> outputs = byteTracker.Update(inputs);

                            foreach (var output in outputs)
                            {
                                int track_id = output.TrackId;
                                TlwhRect rect = (TlwhRect)output.Detection.Rect;
                                Scalar color = palette[track_id % palette.Count];

                                Imgproc.rectangle(rgbaMat, new Point(rect.Left, rect.Top), new Point(rect.Left + rect.Width, rect.Top + rect.Height), color, 2);

                                string label = "ID:" + track_id;
                                int[] baseLine = new int[1];
                                Size labelSize = Imgproc.getTextSize(label, Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, 1, baseLine);

                                float left = rect.Left;
                                float top = Mathf.Max(rect.Top, (float)labelSize.height);
                                Imgproc.rectangle(rgbaMat, new Point(left, top - labelSize.height),
                                    new Point(left + labelSize.width, top + baseLine[0]), color, Core.FILLED);
                                Imgproc.putText(rgbaMat, label, new Point(left, top), Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, Scalar.all(255), 1, Imgproc.LINE_AA);
                            }
                        }
                    }
                }
#endif


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

        private void ResetTrackers()
        {
            if (byteTracker != null)
                byteTracker.Clear();

            if (!disableObjectDetector)
                showObjectDetectorResultToggle.interactable = true;
        }

        private List<Detection> ConvertToByteTrackDetections(Mat results)
        {
            List<Detection> inputs = new List<Detection>();

            if (results.empty() || results.cols() < 6)
                return inputs;

            for (int i = results.rows() - 1; i >= 0; --i)
            {
                float[] box = new float[4];
                results.get(i, 0, box);
                float[] conf = new float[1];
                results.get(i, 4, conf);
                //float[] cls = new float[1];
                //results.get(i, 5, cls);

                float left = box[0];
                float top = box[1];
                float width = box[2] - box[0];
                float height = box[3] - box[1];
                float score = conf[0];

                inputs.Add(new Detection(new TlwhRect(top, left, width, height), score));
            }

            return inputs;
        }

        /// <summary>
        /// Raises the destroy event.
        /// </summary>
        void OnDestroy()
        {
            if (multiSource2MatHelper != null)
                multiSource2MatHelper.Dispose();

#if !UNITY_WSA_10_0
            if (objectDetector != null)
                objectDetector.dispose();
#endif

            if (cts != null)
                cts.Dispose();
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
        /// Raises the reset trackers button click event.
        /// </summary>
        public void OnResetTrackersButtonClick()
        {
            ResetTrackers();
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