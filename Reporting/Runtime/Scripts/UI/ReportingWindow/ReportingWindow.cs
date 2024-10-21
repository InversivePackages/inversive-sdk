#if UNITY_2022_3_OR_NEWER
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
namespace Reporting
{
    public class ReportingWindow : MonoBehaviour
    {
        /// <summary>
        /// XR Values
        /// </summary>
        private GameObject rightController;
        private GameObject leftController;
        private MeshRenderer[] raycastCylinders;

        [SerializeField]
        private InputActionProperty rightControllerTrigger, leftControllerTrigger;

        private Texture2D originalScreenshotTexture;
        private Texture2D screenshotTexture;
        [SerializeField]
        private Image screenshotImage;
        [SerializeField]
        private GameObject negativeOutline;
        [SerializeField]
        private GameObject positiveOutline;
        [SerializeField]
        private TMP_InputField commentInputField;
        [SerializeField]
        private ReportingColorButton defaultSelectedColorButton;
        [SerializeField]
        private ReportingSizeButton defaultSelectedSizeButton;
        [SerializeField]
        private ReportingEvaluationButton defaultSelectedEvaluationButton;
        private bool evaluation;

        private bool isDrawing = false;
        private bool isDrawingVRRight = false;
        private bool isDrawingVRLeft = false;
        private bool isHover = false;
        private Color drawColor; // Color of the drawing
        private int brushSize = 5; // Size of the brush
        private static ReportingWindow instance;

        private int lastXRight = 0;
        private int lastYRight = 0;
        private int lastXLeft = 0;
        private int lastYLeft = 0;
        [SerializeField]
        private int drawLineMaxIterations = 300;
        private bool allowDrawlineRight = false;
        private bool allowDrawlineLeft = false;

        [SerializeField]
        private GraphicRaycaster graphicRaycaster;
        public EventSystem eventSystem;
        private bool canSendComment = true;

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            defaultSelectedColorButton.UpdateColor();
            defaultSelectedSizeButton.UpdateSize();
            defaultSelectedEvaluationButton.UpdateEvaluation();
            if (Utility.IsVR())
            {
                rightController = XRController.GetInstance().GetRightController();
                leftController = XRController.GetInstance().GetLeftController();
                raycastCylinders = XRController.GetInstance().GetRaycastCylinders();
                for (int i = 0; i < raycastCylinders.Length; i++)
                {
                    raycastCylinders[i].enabled = true;
                }
            }
            //Enables the detection of trigger inputs for both controllers
            rightControllerTrigger.action.Enable();
            leftControllerTrigger.action.Enable();
        }
        public static ReportingWindow GetInstance()
        {
            return instance;
        }
        /*********************** UPDATE SCREENSHOT /***********************/
        public void SetScreenshotTexture(Texture2D renderTexture)
        {
            // Copy the original texture
            originalScreenshotTexture = new Texture2D(renderTexture.width, renderTexture.height, renderTexture.format, renderTexture.mipmapCount > 1);
            Graphics.CopyTexture(renderTexture, originalScreenshotTexture);
            // Create a copy for editing
            screenshotTexture = new Texture2D(originalScreenshotTexture.width, originalScreenshotTexture.height, originalScreenshotTexture.format, originalScreenshotTexture.mipmapCount > 1);
            Graphics.CopyTexture(originalScreenshotTexture, screenshotTexture);
            UpdateSprite();
        }
        private void UpdateSprite()
        {
            // Convert the Texture2D to a Sprite
            Sprite screenshotSprite = Sprite.Create(
                screenshotTexture,
                new Rect(0, 0, screenshotTexture.width, screenshotTexture.height),
                new Vector2(0.5f, 0.5f)
            );
            // Assign the Sprite to the Image component
            screenshotImage.sprite = screenshotSprite;
        }
        public void ResetSprite()
        {
            // Reset the screenshot texture to the original
            Graphics.CopyTexture(originalScreenshotTexture, screenshotTexture);
            UpdateSprite();
        }
        /*********************** DRAWING /***********************/
        void Update()
        {
            if (Utility.IsVR())
            {
                if (rightControllerTrigger.action.WasPressedThisFrame()) isDrawingVRRight = true;
                else if (rightControllerTrigger.action.WasReleasedThisFrame()) { isDrawingVRRight = false; allowDrawlineRight = false; };
                if (leftControllerTrigger.action.WasPressedThisFrame()) isDrawingVRLeft = true;
                else if (leftControllerTrigger.action.WasReleasedThisFrame()) { isDrawingVRLeft = false; allowDrawlineLeft = false; };
                if (isDrawingVRRight) DrawVRRight();
                if (isDrawingVRLeft) DrawVRLeft();
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDrawing = true;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    isDrawing = false;
                    allowDrawlineLeft = false;
                }
                if (isDrawing && isHover)
                {
                    Draw();
                }
            }
        }
        public void Draw()
        {
            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(screenshotImage.rectTransform, Input.mousePosition, null, out localMousePos);
            // Calculate the texture coordinates
            Rect rect = screenshotImage.rectTransform.rect;
            float x = (localMousePos.x - rect.x) * screenshotTexture.width / rect.width;
            float y = (localMousePos.y - rect.y) * screenshotTexture.height / rect.height;
            int pixelX = Mathf.Clamp((int)x, 0, screenshotTexture.width - 1);
            int pixelY = Mathf.Clamp((int)y, 0, screenshotTexture.height - 1);
            DrawOnTexture(pixelX, pixelY, false);
        }
        public void DrawVRRight()
        {
            Ray ray = new Ray(rightController.transform.position, rightController.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 10, (1 << LayerMask.NameToLayer("UI")))) // Ray only touches elements in the UI layer
            {
                // Vérifiez si l'impact est sur un élément UI avec un collider
                if (hit.collider != null)
                {
                    // Vérifiez si l'impact est sur le screenshot sur lequel on veut dessiner
                    if (hit.collider.gameObject == screenshotImage.gameObject)
                    {
                        // Convertir la position de l'impact en coordonnées locales de l'UI
                        RectTransform rectTransform = hit.collider.gameObject.GetComponent<RectTransform>();
                        Vector2 localMousePos;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            rectTransform,
                            Camera.main.WorldToScreenPoint(hit.point),
                            Camera.main,
                            out localMousePos
                        );
                        // Calculer les coordonnées de la texture
                        Rect rect = rectTransform.rect;
                        float x = (localMousePos.x - rect.x) * screenshotTexture.width / rect.width;
                        float y = (localMousePos.y - rect.y) * screenshotTexture.height / rect.height;
                        int pixelX = Mathf.Clamp((int)x, 0, screenshotTexture.width - 1);
                        int pixelY = Mathf.Clamp((int)y, 0, screenshotTexture.height - 1);
                        DrawOnTexture(pixelX, pixelY, true);
                    }
                    else
                    {
                        isDrawingVRRight = false;
                    }
                }
            }
            else
            {
                isDrawingVRRight = false;
            }
        }
        public void DrawVRLeft()
        {
            Ray ray = new Ray(leftController.transform.position, leftController.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 10, (1 << LayerMask.NameToLayer("UI")))) // Ray only touches elements in the UI layer
            {
                // Vérifiez si l'impact est sur un élément UI avec un collider
                if (hit.collider != null)
                {
                    // Vérifiez si l'impact est sur le screenshot sur lequel on veut dessiner
                    if (hit.collider.gameObject == screenshotImage.gameObject)
                    {
                        // Convertir la position de l'impact en coordonnées locales de l'UI
                        RectTransform rectTransform = hit.collider.gameObject.GetComponent<RectTransform>();
                        Vector2 localMousePos;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            rectTransform,
                            Camera.main.WorldToScreenPoint(hit.point),
                            Camera.main,
                            out localMousePos
                        );
                        // Calculer les coordonnées de la texture
                        Rect rect = rectTransform.rect;
                        float x = (localMousePos.x - rect.x) * screenshotTexture.width / rect.width;
                        float y = (localMousePos.y - rect.y) * screenshotTexture.height / rect.height;
                        int pixelX = Mathf.Clamp((int)x, 0, screenshotTexture.width - 1);
                        int pixelY = Mathf.Clamp((int)y, 0, screenshotTexture.height - 1);
                        DrawOnTexture(pixelX, pixelY, false);
                    }
                    else
                    {
                        isDrawingVRLeft = false;
                    }
                }
            }
            else
            {
                isDrawingVRLeft = false;
            }
        }
        public void SetIsHover(bool isHover)
        {
            this.isHover = isHover;
        }

        private void DrawCircle(int x, int y)
        {
            int radiusSquared = brushSize * brushSize;
            for (int i = -brushSize; i <= brushSize; i++)
            {
                for (int j = -brushSize; j <= brushSize; j++)
                {
                    // Check if the current pixel is within the circle
                    if (i * i + j * j <= radiusSquared)
                    {
                        if (x + i >= 0 && x + i < screenshotTexture.width && y + j >= 0 && y + j < screenshotTexture.height)
                        {
                            screenshotTexture.SetPixel(x + i, y + j, drawColor);
                        }
                    }
                }
            }
        }

        private void DrawLine(int x0, int y0, int x1, int y1) //uses Bresenham's line algorithm to draw a line from (x0, y0) to (x1, y1)
        {
            int dx = Mathf.Abs(x1 - x0); // The absolute difference in x-coordinates
            int dy = Mathf.Abs(y1 - y0); // The absolute difference in y-coordinates
            int sx = x0 < x1 ? 1 : -1;   // The step direction in the x-axis (either 1 or -1)
            int sy = y0 < y1 ? 1 : -1;   // The step direction in the y-axis (either 1 or -1)
            int err = dx - dy;           // The initial error term. It helps determine whether the next point should be in the x direction or the y direction. The error term is adjusted as we move along the line to keep the line as straight as possible.

            int i = 0; //i allows to make sure the function ends and does not caus performance issues

            while (i < drawLineMaxIterations)
            {
                i++;
                DrawCircle(x0, y0); // Draw the circle at the current point

                if (x0 == x1 && y0 == y1) break; // If the end point is reached, break the loop
                int e2 = 2 * err; // Twice the error term
                if (e2 > -dy)
                {
                    err -= dy; // Decrease the error term by dy
                    x0 += sx;  // Move in the x direction
                }
                if (e2 < dx)
                {
                    err += dx; // Increase the error term by dx
                    y0 += sy;  // Move in the y direction
                }
            }
        }

        private void DrawOnTexture(int x, int y, bool right)
        {
            if (right)
            {
                if (allowDrawlineRight)
                {
                    DrawLine(lastXRight, lastYRight, x, y);
                }
                allowDrawlineRight = true;
                lastXRight = x;
                lastYRight = y;
            }
            else
            {
                if (allowDrawlineLeft)
                {
                    DrawLine(lastXLeft, lastYLeft, x, y);
                }
                allowDrawlineLeft = true;
                lastXLeft = x;
                lastYLeft = y;
            }

            DrawCircle(x, y);

            screenshotTexture.Apply();
        }
        /*********************** BRUSH /***********************/
        public void UpdateBrushSize(int size)
        {
            brushSize = size;
        }
        public void UpdateBrushColor(Color color)
        {
            drawColor = color;
        }
        /*********************** EVALUTATION /***********************/
        public void SetEvaluation(bool evaluation)
        {
            this.evaluation = evaluation;
            if (evaluation) negativeOutline.SetActive(false);
            else positiveOutline.SetActive(false);
        }
        /*********************** VALIDATION /***********************/
        public void Cancel()
        {
            Close();
        }
        public void Send()
        {
            if (canSendComment) {
                canSendComment = false;
                ScreenshotData screenshotData = new ScreenshotData(screenshotImage, evaluation, commentInputField.text);
                ExperienceCreateNotationModel experienceCreateNotationModel = new ExperienceCreateNotationModel();
                experienceCreateNotationModel.IsConform = evaluation;
                experienceCreateNotationModel.Comment = commentInputField.text;
                StartCoroutine(InversiveController.SendNotation(experienceCreateNotationModel, screenshotTexture, isSuccess =>
                {
                    if (isSuccess)
                    {
                        Close();
                    }
                    canSendComment = true;
                }));
            }
        }

        public void Close()
        {
            if (raycastCylinders != null)
            {
                for (int i = 0; i < raycastCylinders.Length; i++)
                {
                    raycastCylinders[i].enabled = false;
                }
            }
            Destroy(screenshotTexture);
            Destroy(transform.parent.gameObject);
            ScreenshotHandler.GetInstance().StopEvent();
        }
        /*********************** UTILITY /***********************/
        Texture2D ConvertSpriteToTexture2D(Sprite sprite)
        {
            // Create a new Texture2D with the same dimensions as the sprite
            Texture2D texture2D = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

            // Apply the pixels from the sprite's texture to the new texture
            Rect rect = sprite.rect;
            texture2D.ReadPixels(rect, 0, 0);
            texture2D.Apply();

            // Optional: it may be useful to set filters and wrap modes
            texture2D.filterMode = FilterMode.Point;
            texture2D.wrapMode = TextureWrapMode.Clamp;

            return texture2D;
        }
    }
    public struct ScreenshotData
    {
        public Image image;
        public bool evaluation;
        public string comment;

        public ScreenshotData(Image image, bool evaluation, string comment)
        {
            this.image = image;
            this.evaluation = evaluation;
            this.comment = comment;
        }
    }
}
#endif