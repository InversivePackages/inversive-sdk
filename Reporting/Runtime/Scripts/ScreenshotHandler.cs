#if UNITY_2022_3_OR_NEWER
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Events;
namespace Reporting
{
    public class ScreenshotHandler : MonoBehaviour
    {
        private const int textureDepth = 24;
        private const string fileExtension = ".png";
        [SerializeField]
        private string prefixScreenshotName = "screenshot_";
        private static ScreenshotHandler instance;
        [SerializeField]
        private GameObject reportingWindowVRPrefab;
        [SerializeField]
        private GameObject reportingWindowWEBGLPrefab;
        private GameObject reportingWindow;
        [SerializeField]
        private UnityEvent startScreenshotEvent;
        [SerializeField]
        private UnityEvent stopScreenshotEvent;
        private Camera screenshotCamera;
        private const string SCREENSHOT_TEXTURE_RESOURCE_PATH = "Reporting/Textures/CameraRenderTexture";
        private void Awake() {
            instance = this;
        }
        private Camera GetScreenshotCamera()
        {
            if (screenshotCamera == null)
            {
                if (Utility.IsVR()) screenshotCamera = XRController.GetInstance().GetScreenshotCamera();
                else screenshotCamera = Camera.main;
            }
            return screenshotCamera;
        }
        public static ScreenshotHandler GetInstance() {
            return instance;
        }
        public void TakeScreenshot(int width, int height) {
            StartCoroutine(TakeScreenshotCoroutine(width, height));
        }
        public void ResetScreenshotTexture()
        {
            RenderTexture texture = Resources.Load<RenderTexture>(SCREENSHOT_TEXTURE_RESOURCE_PATH);
            screenshotCamera.targetTexture = texture;
        }
        /// <summary>
        /// Start Event is called when the Screenshot process starts
        /// </summary>
        public void StartEvent()
        {
            startScreenshotEvent.Invoke();
        }
        public UnityEvent GetStartEvent()
        {
            return startScreenshotEvent;
        }
        /// <summary>
        /// Stop Event is called when the Screenshot process take end
        /// </summary>
        public void StopEvent()
        {
            stopScreenshotEvent.Invoke();
        }
        public UnityEvent GetStopEvent()
        {
            return stopScreenshotEvent;
        }
        private IEnumerator TakeScreenshotCoroutine(int width, int height) {
            yield return new WaitForEndOfFrame();

            RenderTexture renderTexture = new RenderTexture(width, height, textureDepth);
            GetScreenshotCamera().targetTexture = renderTexture;
            GetScreenshotCamera().Render();

            RenderTexture.active = renderTexture;
            Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshot.Apply();

            GetScreenshotCamera().targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture);
            StartEvent();
            CreateReportingWindow(screenshot);
        }
        private void CreateReportingWindow(Texture2D screenshot) {
            if (reportingWindow) Destroy(reportingWindow);
            if (Utility.IsVR()) {
                XRController xRController = XRController.GetInstance();
                Vector3 location = xRController.GetScreenshotEnvironmentLocation();
                location += xRController.transform.forward*2;
                reportingWindow = Instantiate(reportingWindowVRPrefab, location, Quaternion.identity);
            } else {
                reportingWindow = Instantiate(reportingWindowWEBGLPrefab);
            }
            reportingWindow.transform.GetChild(0).GetComponent<ReportingWindow>().SetScreenshotTexture(screenshot);
        }
        public void SaveScreenshot(Texture2D screenshot) {
            byte[] bytes = screenshot.EncodeToPNG();
            // Format the current date and time
            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
#if UNITY_EDITOR
            // Unity Editor behaviour
            string fileName = string.Concat(prefixScreenshotName, timestamp, fileExtension);
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(filePath, bytes);
            CreateLogMessage(filePath);
#elif UNITY_WEBGL
            // WebGL behaviour
            string encodedText = System.Convert.ToBase64String(bytes);
            Application.ExternalCall("DownloadScreenshot", encodedText, fileName);

#elif UNITY_ANDROID
            // Android behaviour
            string fileName = string.Concat(prefixScreenshotName, timestamp, fileExtension);
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(filePath, bytes);
            CreateLogMessage(filePath);
#endif
        }
        private void CreateLogMessage(string filePath) {
            string msg = $"Screenshot saved to: {filePath}";
            string msgColorHex = ColorUtility.ToHtmlStringRGB(Color.green);
            Debug.Log($"<color=#{msgColorHex}>{"Reporting: "}</color>{msg}");
        }
    }
}
#endif