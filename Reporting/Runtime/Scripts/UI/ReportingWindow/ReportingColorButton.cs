#if UNITY_2022_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Reporting
{
    public class ReportingColorButton : MonoBehaviour
    {
        private Color color;
        private GameObject outlineGameObject;
        private static List<ReportingColorButton> instances = new List<ReportingColorButton>();
        private void Awake() {
            instances.Add(this);
            color = GetComponent<Image>().color;
            outlineGameObject = transform.GetChild(0).gameObject;
            GetComponent<Button>().onClick.AddListener(UpdateColor);
        }
        public void UpdateColor() {
            ReportingWindow.GetInstance().UpdateBrushColor(color);
            foreach(ReportingColorButton reportingColorButton in instances) {
                reportingColorButton.ActiveOutline(reportingColorButton==this);
            }
        }
        public void ActiveOutline(bool activation) {
            outlineGameObject.SetActive(activation);
        }
        public void OnDestroy() {
            instances.Remove(this);
        }
    }
}
#endif