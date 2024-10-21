#if UNITY_2022_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Reporting
{
    public class ReportingEvaluationButton : MonoBehaviour
    {
        [SerializeField]
        private bool evaluationValue;
        private GameObject outlineGameObject;
        private static List<ReportingEvaluationButton> instances = new List<ReportingEvaluationButton>();
        private void Awake() {
            instances.Add(this);
            outlineGameObject = transform.GetChild(0).gameObject;
            GetComponent<Button>().onClick.AddListener(UpdateEvaluation);
        }
        public void UpdateEvaluation() {
            ReportingWindow.GetInstance().SetEvaluation(evaluationValue);
            foreach(ReportingEvaluationButton reportingEvaluationButton in instances) {
                reportingEvaluationButton.ActiveOutline(reportingEvaluationButton==this);
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