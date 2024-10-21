#if UNITY_2022_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Reporting
{
    public class ReportingSizeButton : MonoBehaviour
    {
        [SerializeField]
        private int size;
        private GameObject outlineGameObject;
        private static List<ReportingSizeButton> instances = new List<ReportingSizeButton>();
        private void Awake() {
            instances.Add(this);
            outlineGameObject = transform.GetChild(0).gameObject;
            GetComponent<Button>().onClick.AddListener(UpdateSize);
        }
        public void UpdateSize() {
            ReportingWindow.GetInstance().UpdateBrushSize(size);
            foreach(ReportingSizeButton reportingSizeButton in instances) {
                reportingSizeButton.ActiveOutline(reportingSizeButton==this);
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