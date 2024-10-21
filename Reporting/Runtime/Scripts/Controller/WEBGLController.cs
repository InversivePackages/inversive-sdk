#if UNITY_2022_3_OR_NEWER
using UnityEngine;
namespace Reporting
{
    public class WEBGLController : MonoBehaviour
    {
        private const string CANVAS_START_REPORTING_RESOURCE_PATH = "Reporting/UI/CanvasStartReporting";
        private GameObject canvasStartReporting;
        private void Awake()
        {
            GameObject resource = Resources.Load<GameObject>(CANVAS_START_REPORTING_RESOURCE_PATH);
            canvasStartReporting = Instantiate(resource);
        }
        void TakeScreenshot() {
            ScreenshotHandler.GetInstance().TakeScreenshot(1280, 720);
        }
        void Update() {
            if (Input.GetKeyDown(KeyCode.F8)) {
                TakeScreenshot();
            }
        }
        private void OnDestroy()
        {
            Destroy(canvasStartReporting);
        }
    }
}
#endif