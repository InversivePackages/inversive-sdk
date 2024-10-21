#if UNITY_2022_3_OR_NEWER
using Reporting;
using UnityEngine;
using UnityEngine.UI;
public class StartReporting : MonoBehaviour
{
    [SerializeField]
    private Image outline;

    private void Start()
    {
        VisualFeedback(false);
    }

    public void VisualFeedback(bool show)
    {
        if (Utility.IsVR()) //The outline only exists in VR
        {
            outline.enabled = show;
        }
    }

    public void TakeScreenshot()
    {
        ScreenshotHandler.GetInstance().TakeScreenshot(1280, 720);
    }
}
#endif