#if UNITY_2022_3_OR_NEWER
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
namespace Reporting
{
    public class UIVRInteracter : MonoBehaviour
    {
        private GameObject rightController;
        private GameObject leftController;

        [SerializeField]
        private InputActionProperty rightControllerTrigger, leftControllerTrigger;
        [SerializeField, Tooltip("Allows the user to trigger a screenshot by pressing and releasing the left trigger")]
        private bool allowLeftControllerScreenshot;
        [SerializeField]
        private StartReporting startReporting;
        private bool blockScreenshot = false;
        private Coroutine awaitingScreenshotCroutine;

        private AudioSource _audioSource;
        public bool disable = false;

        private void Awake()
        {
            //Enables the detection of trigger inputs for both controllers
            rightControllerTrigger.action.Enable();
            leftControllerTrigger.action.Enable();
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();

            UnityEvent startEvent = ScreenshotHandler.GetInstance().GetStartEvent();
            startEvent.AddListener(() => blockScreenshot = true);
            UnityEvent stopEvent = ScreenshotHandler.GetInstance().GetStopEvent();
            stopEvent.AddListener(() => StartCoroutine(AllowScreenshotAfterDelay(2.0f)));

            if (Utility.IsVR())
            {
                rightController = XRController.GetInstance().GetRightController();
                leftController = XRController.GetInstance().GetLeftController();
            }

            rightControllerTrigger.action.performed +=
                context =>
                {
                    OnClickVR(rightController);
                };

            leftControllerTrigger.action.performed +=
                context =>
                {
                    OnClickVR(leftController);
                    if (allowLeftControllerScreenshot && !blockScreenshot && !disable)
                    {
                        if (awaitingScreenshotCroutine != null) StopCoroutine(awaitingScreenshotCroutine);
                        awaitingScreenshotCroutine = StartCoroutine(AwaitingScreenshot(true));
                    };
                };
        }

        private IEnumerator AllowScreenshotAfterDelay(float delay) //Adding a delay before allowing to take a new screenshot avoids taking an immediate new screenshot when using the left trigger to exit the reporting window
        {
            yield return new WaitForSeconds(delay);
            blockScreenshot = false;
        }

        private void OnClickVR(GameObject controller)
        {
            Ray ray = new Ray(controller.transform.position, controller.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Vérifiez si l'impact est sur un élément UI avec un collider
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == startReporting.gameObject && !blockScreenshot && !disable)
                    {
                        if (awaitingScreenshotCroutine != null) StopCoroutine(awaitingScreenshotCroutine);
                        awaitingScreenshotCroutine = StartCoroutine(AwaitingScreenshot(false));
                    }
                    // Si c'est un bouton, on active le onClick event associé et on jour un SFX
                    else if (hit.collider.gameObject.TryGetComponent<UnityEngine.UI.Button>(out UnityEngine.UI.Button btn))
                    {
                        _audioSource.Play();
                        btn.onClick.Invoke();
                    }
                }
            }
        }

        private IEnumerator AwaitingScreenshot(bool leftController)
        {
            startReporting.VisualFeedback(true);
            //With the left controller, the screenshot is taken when the user releases the trigger
            if (leftController)
            {
                while (!leftControllerTrigger.action.WasReleasedThisFrame())
                {
                    yield return null;
                }
                startReporting.TakeScreenshot();
                startReporting.VisualFeedback(false);
            }
            //With the right controller, the screenshot is taken when the user releases the trigger, only if they still aim at the screenshot button.
            else
            {
                while (!rightControllerTrigger.action.WasReleasedThisFrame())
                {
                    Ray ray = new Ray(rightController.transform.position, rightController.transform.forward);
                    if (!Physics.Raycast(ray, out RaycastHit hit))
                    {
                        startReporting.VisualFeedback(false);
                        yield break;
                    }
                    else if (hit.collider == null)
                    {
                        startReporting.VisualFeedback(false);
                        yield break;
                    }
                    else if (hit.collider.gameObject != startReporting.gameObject)
                    {
                        startReporting.VisualFeedback(false);
                        yield break;
                    }
                    yield return null;
                }
                startReporting.TakeScreenshot();
                startReporting.VisualFeedback(false);
            }
        }
    }
}
#endif