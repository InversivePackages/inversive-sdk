#if UNITY_2022_3_OR_NEWER
using UnityEngine;
using UnityEngine.Events;
namespace Reporting
{
    public class XRController : MonoBehaviour
    {
        private static XRController instance;
        [SerializeField]
        private GameObject rightController;
        [SerializeField]
        private GameObject leftController;
        [SerializeField]
        private MeshRenderer[] raycastCylinders;
        private Vector3 initalLocation;
        [SerializeField]
        private GameObject cameraCanvas;
        [SerializeField]
        private Camera screenshotCamera;
        [SerializeField]
        private string[] layersToIgnore;
        private const string ISOLATION_ENVIRONMENT_RESOURCE_PATH = "Reporting/ScreenshotIsolation";
        private GameObject isolationEnvironment;
        /// <summary>
        /// The location where the screenshot process will be loaded in the scene.
        /// </summary>
        [SerializeField]
        private Vector3 screenshotEnvironmentLocation = new Vector3(0,-100,0);

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            UnityEvent startEvent = ScreenshotHandler.GetInstance().GetStartEvent();
            startEvent.AddListener(TeleportToScreenshotEnvironement);
            startEvent.AddListener(() => ActivationCameraCanvas(false));
            startEvent.AddListener(() => ActivationIsolationEnvironment(true));
            UnityEvent stopEvent = ScreenshotHandler.GetInstance().GetStopEvent();
            stopEvent.AddListener(TeleportToInitialPosition);
            stopEvent.AddListener(() => ActivationCameraCanvas(true));
            stopEvent.AddListener(() => ActivationIsolationEnvironment(false));
            stopEvent.AddListener(ResetScreenshotTexture);
            for (int i = 0; i < raycastCylinders.Length; i++) //The raycast cylinders are hidden until the user opens the reporting window
            {
                raycastCylinders[i].enabled = false;
            }

            SetScreenshotCameraCullingMask();
        }
        // Method to set the culling mask based on the layers to ignore
        public void SetScreenshotCameraCullingMask()
        {
            // Start with a culling mask that includes all layers
            int cullingMask = ~0;

            foreach (string layerName in layersToIgnore)
            {
                int layer = LayerMask.NameToLayer(layerName);
                if (layer != -1)
                {
                    // Invert the bit of the layer to ignore it
                    cullingMask &= ~(1 << layer);
                }
                else
                {
                    Debug.LogWarning("Layer \"" + layerName + "\" does not exist.");
                }
            }

            // Set the culling mask of the target camera
            screenshotCamera.cullingMask = cullingMask;
        }
        public void ResetScreenshotTexture()
        {
            ScreenshotHandler.GetInstance().ResetScreenshotTexture();
        }
        public GameObject GetRightController() { return rightController; }
        public GameObject GetLeftController() {  return leftController; }
        public MeshRenderer[] GetRaycastCylinders() { return raycastCylinders; }
        public static XRController GetInstance()
        {
            return instance;
        }
        public Camera GetScreenshotCamera()
        {
            return screenshotCamera;
        }
        public void ActivationIsolationEnvironment(bool activation)
        {
            if(activation)
            {
                if (isolationEnvironment) Destroy(isolationEnvironment);
                isolationEnvironment = Resources.Load<GameObject>(ISOLATION_ENVIRONMENT_RESOURCE_PATH);
                isolationEnvironment = Instantiate(isolationEnvironment, transform.position, Quaternion.identity);
            }
            else
            {
                if (isolationEnvironment) Destroy(isolationEnvironment);
            }
        }
        public void ActivationCameraCanvas(bool activation)
        {
            cameraCanvas.SetActive(activation);
        }
#region Teleportation
        public Vector3 GetScreenshotEnvironmentLocation()
        {
            return screenshotEnvironmentLocation;
        }
        public void TeleportToScreenshotEnvironement()
        {
            Transform controllerTransform = MainController.GetInstance().GetCurrentController().transform;
            initalLocation = controllerTransform.position;
            controllerTransform.position = screenshotEnvironmentLocation;
        }
        public void TeleportToInitialPosition()
        {
            Transform controllerTransform = MainController.GetInstance().GetCurrentController().transform;
            controllerTransform.position = initalLocation;
        }
#endregion
    }
}
#endif