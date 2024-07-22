using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Inversive.SDK;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Level[] Levels;
    [SerializeField] GameObject ResetScreen, End;
    [SerializeField] public GameObject NextButton, PreviousButton;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] TMP_Text GlobalScore;
    [SerializeField] TMP_Text Feedback;
    [HideInInspector]
    public List<ToggleButton> selectedAnswers = new();
    [HideInInspector]
    public Level CurrentLevel;
    private bool AnswerChecked = false;

    float startingTime = 8.0f;
    float currentTime = 0.0f;
    int currentLevelIndex = 0;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        //Initialize your sdk & retrieve your session informations
        InversiveSdk.Init((x) =>
        {
            Debug.Log($"Initialized & session Id : {x}");
        });
    }

    private void Start()
    {
        currentTime = startingTime;
    }

    private void Update()
    {
        Timer();
    }

    private void Timer()
    {
        if (countdownText.gameObject.activeSelf)
        {
            currentTime -= 1 * Time.deltaTime;
            countdownText.text = currentTime.ToString("0");

            if (countdownText.text == "4")
                countdownText.color = Color.red;

            if (currentTime <= 0)
            {
                currentTime = 0;
                if (!AnswerChecked)
                    CheckAnswer();
            }
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CheckAnswer()
    {
        AnswerChecked = true;
        var level = Levels[currentLevelIndex];
        var values = new List<string>();
        var actionName = currentLevelIndex == 0 ? "FirstQuestion" : "SecondQuestion";
        foreach (var selectedAnswer in selectedAnswers)
        {
            selectedAnswer.IsCorrect = level.correctAnswers.Contains(selectedAnswer);
            selectedAnswer.IsChecking = true;
            values.Add(selectedAnswer.gameObject.GetComponentInChildren<TMP_Text>().text);
        }

        //Report that the action has been performed and retrieve the score for that action
        InversiveSdk.ExecuteAction("FirstChapter", actionName, values, (score) =>
        {
            Debug.Log($"Action score : {score}");
        });

        foreach (var correctAnswer in level.correctAnswers)
            if (!selectedAnswers.Contains(correctAnswer))
            {
                correctAnswer.IsCorrect = true;
                correctAnswer.IsChecking = true;
            }
        NextButton.SetActive(true);
    }

    public void StartGame()
    {
        //Signals that the session has been started
        InversiveSdk.StartExperience((IsSuccess) =>
        {
            Debug.Log($"Started Experience succeed ? : {IsSuccess}");
        });
        selectedAnswers.Clear();
        NextButton.SetActive(false);
        PreviousButton.SetActive(false);
        nextQuestion(true);
    }

    public void nextQuestion(bool start = false)
    {
        countdownText.color = Color.white;
        NextButton.SetActive(false);
        selectedAnswers.Clear();
        if (currentLevelIndex + 1 != Levels.Length)
        {
            AnswerChecked = false;
            Levels[currentLevelIndex].gameObject.SetActive(false);
            if (!start)
                currentLevelIndex++;

            Levels[currentLevelIndex].gameObject.SetActive(true);
            CurrentLevel = Levels[currentLevelIndex];
            currentTime = startingTime;
            countdownText.gameObject.SetActive(true);
        }
        else
        {
            //Report session ended
            InversiveSdk.End((IsSuccess) =>
            {
                Debug.Log($"Ended Experience succeed ? : {IsSuccess}");
                if (IsSuccess)
                {
                    GlobalScore.text = $"Score : {InversiveSdk.GetDisplayedGlobalScore()}";
                    Feedback.text = InversiveSdk.GetGlobalScore() >= InversiveSdk.GetWinScore() ? "you succeeded" : "you failed";
                }
            });
            End.SetActive(true);
            Levels[currentLevelIndex].gameObject.SetActive(false);
        }
    }

    public void Retry()
    {
        //Resets session data to start from the beginning
        InversiveSdk.Retry((x) =>
        {
            Debug.Log($"Retry Experience succeed ? : {x}");
        });
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void OnToggleChange(ToggleButton toggle)
    {
        if (toggle.Checked)
            selectedAnswers.Add(toggle);
        else
            selectedAnswers.Remove(toggle);
    }

}
