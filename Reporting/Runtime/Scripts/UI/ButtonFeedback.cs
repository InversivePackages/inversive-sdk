#if UNITY_2022_3_OR_NEWER
using UnityEngine;
using UnityEngine.UI;

public class ButtonFeedback : MonoBehaviour
{
    private Button btn;
    private Color defaultColor;
    [SerializeField]
    private Color highlightedColor = Color.white;

    private void Awake()
    {
        btn = GetComponent<Button>();
        defaultColor = btn.colors.normalColor;
    }

    private void OnEnable() //resets to the default color each time the menu including the button is reactivated
    {
        ColorBlock cb = btn.colors;
        cb.normalColor = defaultColor;
        btn.colors = cb;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "RaycastCylinder")
        {
            ColorBlock cb = btn.colors;
            cb.normalColor = highlightedColor;
            btn.colors = cb;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "RaycastCylinder")
        {
            ColorBlock cb = btn.colors;
            cb.normalColor = defaultColor;
            btn.colors = cb;
        }
    }
}
#endif