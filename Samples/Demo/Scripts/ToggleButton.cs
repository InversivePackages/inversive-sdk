using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ToggleButton : MonoBehaviour, IPointerClickHandler
{
    public ToggleEvent CheckedChanged = new ToggleEvent();
    Image _image;
    Color _originalColor;
    bool _checked;

    [SerializeField] Color _checkedColor;
    [SerializeField] Color[] colors;
    private int currentColorIndex;
    private int targetColorIndex;
    private float duration = 2;
    [HideInInspector]
    public bool AnswerChecked = false;
    [HideInInspector]
    public bool IsChecking = false;
    [HideInInspector]
    public bool IsCorrect = false;


    [SerializeField]
    public bool Checked
    {
        get
        {
            return _checked;
        }
        set
        {
            if (_checked != value)
            {
                _checked = value;
                UpdateVisual();
                CheckedChanged.Invoke(this);
            }
        }
    }

    void Start()
    {
        _image = GetComponent<Image>();
        _originalColor = _image.color;
    }

    void Update()
    {
        if (IsChecking)
            StartCoroutine(ShowResult(IsCorrect));
    }

    private void UpdateVisual()
    {
        _image.color = Checked ? _checkedColor : _originalColor;
    }

    public IEnumerator ShowResult(bool isCorrect)
    {
        IsChecking = false;
        float time = 0;
        currentColorIndex = Checked ? 1 : 0;
        if (isCorrect)
            targetColorIndex = 2;
        else
            targetColorIndex = 3;

        while (time < duration)
        {
            _image.color = Color.Lerp(colors[currentColorIndex], colors[targetColorIndex], time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        _image.color = colors[targetColorIndex];
        yield return null;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Checked)
        {
            if (GameManager.Instance.selectedAnswers.Count < GameManager.Instance.CurrentLevel.maxAnswerCount)
            {
                Checked = !Checked;
            }
        }
        else
        {
            Checked = !Checked;
        }

    }

    [Serializable]
    public class ToggleEvent : UnityEvent<ToggleButton>
    {
    }
}
