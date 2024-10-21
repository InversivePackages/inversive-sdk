#if UNITY_2022_3_OR_NEWER
using UnityEngine;
namespace Reporting
{
    public class MainController : MonoBehaviour
    {
        private GameObject currentController;
        private const string VR_CONTROLLER_RESOURCE_PATH = "Reporting/Controllers/VRController";
        private const string WEBGL_CONTROLLER_RESOURCE_PATH = "Reporting/Controllers/WEBGLController";
        private static MainController instance;
        public void Awake() {
            instance = this;
        }
        public static MainController GetInstance() {
            return instance;
        }
        private void Start() {
            string fileLocation;
            if (Utility.IsVR())
                fileLocation = VR_CONTROLLER_RESOURCE_PATH;
            else
                fileLocation = WEBGL_CONTROLLER_RESOURCE_PATH;
            GameObject resource = Resources.Load<GameObject>(fileLocation);
            currentController = Instantiate(resource, transform);
            StartCoroutine(InversiveController.CheckSession(isSuccess => {
                if (!isSuccess)
                {
                    if (Utility.IsVR())
                    {
                        XRController xrController = currentController.GetComponent<XRController>();
                        UIVRInteracter uiVRInteracter = currentController.GetComponent<UIVRInteracter>();
                        xrController.ActivationCameraCanvas(false);
                        uiVRInteracter.disable = true;
                    }
                }
            }));
        }
        public GameObject GetCurrentController() {
            return currentController;
        }
        private void OnDestroy()
        {
            Destroy(currentController);
        }

    }
}
#endif