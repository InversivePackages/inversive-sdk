using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class InversiveExperience
{

    #region Private Properties

    private static int LastActionScore = 0;

    private static int GlobalScore = 0;

    private static string DisplayedGlobalScore = string.Empty;

    private static ExperienceSessionModel ExperienceSession = null;

    private static ExperienceSessionSummaryModel ExperienceSummary = null;

    private static string SessionId = string.Empty;

    #endregion

    #region Public Properties

    /// <summary>
    /// Retrieves the scoring type of the experience.
    /// Possible scoring types:
    /// - Hundred: Represents scoring based on a hundred-point scale.
    /// - Ten: Represents scoring based on a ten-point scale.
    /// - Twenty: Represents scoring based on a twenty-point scale.
    /// - Letter: Represents scoring using letters (e.g., A, B, C).
    /// </summary>
    /// <returns>The scoring type as a ScoringEnum.</returns>
    public static ScoringEnum GetScoringType()
    {
        return InversiveService.GetExperience().ScoringType;
    }

    /// <summary>
    /// Retrieves the model of the current chapter in the user's experience.
    /// </summary>
    /// <returns>The ExperienceChapterModel representing the current chapter.</returns>
    public static ExperienceChapterModel GetCurrentChapter()
    {
        return ExperienceSession.CurrentChapter;
    }

    /// <summary>
    /// Retrieves the model of the last action performed by the user.
    /// </summary>
    /// <returns>The ExperienceActionModel representing the last action.</returns>
    public static ExperienceActionModel GetLastAction()
    {
        return ExperienceSession.LastAction;
    }

    /// <summary>
    /// Recovers the overall score achieved in the experience.
    /// The global score is bounded between 0 and 100.
    /// </summary>
    /// <returns>An int representing the global score.</returns>
    public static int GetGlobalScore()
    {
        return GlobalScore;
    }

    /// <summary>
    /// Retrieves the global score with the specified scoring applied.
    /// </summary>
    /// <returns>
    /// If scoring type is 'Hundred', returns the GlobalScore as a string.
    /// If scoring type is 'Ten', returns the GlobalScore divided by 10 as a string.
    /// If scoring type is 'Twenty', returns the GlobalScore divided by 5 and rounded as a string.
    /// If scoring type is 'Letter', maps the GlobalScore to a corresponding letter grade (A, B, C, D, E, F).
    /// </returns>
    public static string GetDisplayedGlobalScore()
    {
        return DisplayedGlobalScore;
    }

    /// <summary>
    /// Retrieves the score of the last action performed by the user.
    /// </summary>
    /// <returns>The score of the last action as an integer.</returns>
    public static int GetLastActionScore()
    {
        return LastActionScore;
    }

    /// <summary>
    /// Retrieves the win score related to the user's experience.
    /// </summary>
    /// <returns>The win score as an integer.</returns>
    public static int GetWinScore()
    {
        return InversiveService.GetExperience().WinScore;
    }

    /// <summary>
    /// Retrieves a list of ExperienceChapterModel objects representing chapters in the user's experience.
    /// </summary>
    /// <returns>A list of ExperienceChapterModel objects.</returns>
    public static List<ExperienceChapterModel> GetChapters()
    {
        return InversiveService.GetExperience().Chapters;
    }

    /// <summary>
    /// This method retrieves a JSON string that has been previously saved in the session.
    /// </summary>
    /// <returns>
    /// A string containing the JSON data saved in the session.
    /// </returns>
    public static string GetSavedJson()
    {
        // Accesses the 'Json' property of the 'ExperienceSession' class to retrieve the saved JSON string.
        return ExperienceSession.Json;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Retrieves intent data if running on Android platform to obtain the session ID from an external source.
    /// </summary>
    /// <remarks>
    /// This method fetches intent data specific to Android, used for obtaining session information.
    /// </remarks>
    /// <returns>The intent data as a string containing the session ID or an empty string if unavailable.</returns>
    private static string getIntentData()
    {
#if (!UNITY_EDITOR && UNITY_ANDROID)
    return CreatePushClass (new AndroidJavaClass ("com.unity3d.player.UnityPlayer"));
#endif
        return string.Empty;
    }

    private static string CreatePushClass(AndroidJavaClass UnityPlayer)
    {
#if UNITY_ANDROID
    AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
    AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject> ("getIntent");
    AndroidJavaObject extras = GetExtras (intent);

    if (extras != null) {
            Debug.Log("we have some extras");
        string ex = GetProperty (extras, "sessionId");
            if(!string.IsNullOrEmpty(ex))
            {
                Debug.Log(ex);
                return ex;
            }
    }
#endif
        return string.Empty;
    }

    private static AndroidJavaObject GetExtras(AndroidJavaObject intent)
    {
        AndroidJavaObject extras = null;
        try
        {
            extras = intent.Call<AndroidJavaObject>("getExtras");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return extras;
    }
    private static string GetProperty(AndroidJavaObject extras, string name)
    {
        string s = string.Empty;
        try
        {
            s = extras.Call<string>("getString", name);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return s;
    }

    /// <summary>
    /// Constructs a URI based on the API base URL and a provided relative path.
    /// </summary>
    /// <param name="relativePath">The relative path to be appended to the base URL.</param>
    /// <returns>A Uri object representing the composed URL.</returns>
    private static Uri GetUri(string relativePath)
    {
        var baseUri = new Uri(InversiveUtilities.GetApiUrl());
        return new Uri(baseUri, relativePath);
    }

    /// <summary>
    /// Retrieves the session ID based on the platform the application is running on.
    /// </summary>
    /// <remarks>
    /// The method retrieves the session ID from various sources depending on the platform:
    /// - In the Unity Editor, it retrieves an access token from InversiveService.
    /// - For WebGL builds, it parses the session ID from the URL parameters.
    /// - On Android, it attempts to retrieve the session ID from intent data.
    /// - For other platforms, it retrieves the session ID from command-line arguments.
    /// </remarks>
    /// <returns>The session ID as a string or an empty string if not found.</returns>
    private static string GetSessionId()
    {
#if UNITY_EDITOR
        return string.Empty;
#elif UNITY_WEBGL
            string URL = Application.absoluteURL;
            int lastIndex = URL.LastIndexOf('/');
            string paramsField = URL.Substring(lastIndex + 1);
            return paramsField;
#elif UNITY_ANDROID
            if(getIntentData() is string sessionId && !string.IsNullOrEmpty(sessionId))
            {
                return sessionId;
            }
            else
            {
                return string.Empty;
            }
#else
            string[] args = Environment.GetCommandLineArgs();
            var session = string.Empty;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-session")
                {
                    session = args[i + 1];
                    break;
                }
            }
            return session;
#endif
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the user session.
    /// Init() must be called before using anything else.
    /// </summary>
    /// <param name="callback">Callback function invoked with the session ID upon successful initialization.</param>
    /// <returns>An IEnumerator coroutine.</returns>
    public static IEnumerator Init(Action<string> callback)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(SessionId))
        {
            using (var request = UnityWebRequest.Get(GetUri($"Account/get-session")))
            {
                request.SetRequestHeader("AccessToken", InversiveService.GetAccessToken());
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(InversiveUtilities.CallFailedMessage($"Retrieved session id failed : {request.error}"));
                    ExperienceSession = new ExperienceSessionModel()
                    {
                        Id = Guid.NewGuid().ToString()
                    };
                    callback(ExperienceSession.Id);
                }
                else
                {
                    Debug.Log(InversiveUtilities.SuccessMessage($"Retrieved session id successfully !"));
                    SessionId = request.downloadHandler.text;
                }
            }
        }
#else
        SessionId = GetSessionId();
        Debug.Log(InversiveUtilities.Message($"Get Session Id  : {SessionId}"));
#endif

        if (InversiveService.GetExperienceSession() != null)
            PlayerPrefs.DeleteKey("ExperienceSession");

        if (!string.IsNullOrEmpty(SessionId))
        {
            using (var request = UnityWebRequest.Get(GetUri($"Initialize?session={SessionId}")))
            {
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(InversiveUtilities.CallFailedMessage($"Init() Failed : {request.error}"));
                    ExperienceSession = new ExperienceSessionModel()
                    {
                        Id = Guid.NewGuid().ToString()
                    };
                    callback(ExperienceSession.Id);
                }
                else
                {
                    Debug.Log(InversiveUtilities.SuccessMessage($"Initialized successfully !"));
                    ExperienceSession = JsonConvert.DeserializeObject<ExperienceSessionModel>(request.downloadHandler.text);
                    callback(ExperienceSession.Id);
                }
            }
        }
        else
        {
            Debug.LogError(InversiveUtilities.CallFailedMessage($"Initialize() Failed : Session Id not found"));
            callback(null);
        }
    }

    /// <summary>
    /// Starts the session by setting the start date.
    /// </summary>
    /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
    /// <returns>An IEnumerator coroutine.</returns>
    public static IEnumerator Start(Action<bool> callback)
    {
        ExperienceSession.StartDate = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(SessionId))
        {
            using (var request = UnityWebRequest.Get(GetUri($"Manage/start?session={SessionId}")))
            {
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(InversiveUtilities.CallFailedMessage($"Start() Failed :  {request.error}"));
                    callback(false);
                }
                else
                {
                    Debug.Log(InversiveUtilities.SuccessMessage($"StartDate : {request.downloadHandler.text}"));
                    callback(true);
                }
            }
        }
        else
        {
            Debug.LogError(InversiveUtilities.CallFailedMessage($"Start() Failed : Session Id not found"));
            callback(false);
        }
    }

    /// <summary>
    /// Executes a specific action within a chapter of the experience.
    /// </summary>
    /// <param name="chapterName">Name of the chapter containing the action.</param>
    /// <param name="actionName">Name of the action to be executed.</param>
    /// <param name="values">Values associated with the action. Sent as List of String</param>
    /// <param name="callback">Callback function invoked with the calculated score upon completion.</param>
    /// <returns>An IEnumerator coroutine.</returns>
    public static IEnumerator ExecuteAction(string chapterName, string actionName, List<string> values, Action<int?> callback)
    {
        if (ExperienceSession != null)
        {
            var experienceModel = ExperienceSession.Experience;

            if (experienceModel != null && experienceModel.Chapters.Count > 0)
            {
                var chapter = experienceModel.Chapters.Where(x => x.Name == chapterName).FirstOrDefault();
                if (experienceModel.Chapters?.Where(x => x.Name == chapterName)?.FirstOrDefault() != null)
                {
                    if (chapter.Actions.Where(x => x.Name == actionName)?.FirstOrDefault() != null)
                    {
                        var action = chapter.Actions.Where(x => x.Name == actionName).FirstOrDefault();
                        var newSessionAction = new ExperienceSessionActionModel()
                        {
                            RelatedAction = action,
                            Values = values,
                            Score = 0
                        };
                        ExperienceSession.LastAction = action;
                        LastActionScore = newSessionAction.Score;
                        if (!string.IsNullOrEmpty(SessionId))
                        {
                            var data = JsonConvert.SerializeObject(newSessionAction);
                            using (var request = UnityWebRequest.Put(GetUri($"Action?session={SessionId}"), data))
                            {
                                request.method = "POST";
                                request.SetRequestHeader("Content-Type", "application/json");
                                yield return request.SendWebRequest();
                                if (request.result == UnityWebRequest.Result.Success)
                                {
                                    Debug.Log(InversiveUtilities.SuccessMessage("Execute action on remote successfully !"));
                                    ExperienceSessionActionSummaryModel summaryModel = JsonConvert.DeserializeObject<ExperienceSessionActionSummaryModel>(request.downloadHandler.text);
                                    GlobalScore = summaryModel.GlobalScore;
                                    DisplayedGlobalScore = InversiveUtilities.ReturnGlobalScore(summaryModel.GlobalScore, GetScoringType());
                                    callback(int.Parse(summaryModel.ActionScore.ToString()));
                                }
                                else
                                {
                                    if (request.responseCode == 422)
                                        Debug.LogError(InversiveUtilities.ParsingFailedMessage($"ExecuteAction() Failed : {request.error} \n\n Message : {request.downloadHandler.text}"));
                                    else
                                        Debug.LogError(InversiveUtilities.CallFailedMessage($"ExecuteAction() Failed : {request.error} \n\n Message : {request.downloadHandler.text}"));
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError(InversiveUtilities.CallFailedMessage($"ExecuteAction() Failed :  Session Id not found"));
                            callback(null);
                        }
                    }
                    else
                        Debug.LogError(InversiveUtilities.NotFoundMessage($"ExecuteAction() Failed : No actions named {actionName} exist in the {chapterName} chapter"));
                }
                else
                    Debug.LogError(InversiveUtilities.NotFoundMessage($"ExecuteAction() Failed : No chapters named {chapterName} found"));
            }
            else
                Debug.LogError(InversiveUtilities.NotFoundMessage($"ExecuteAction() Failed : No chapters found"));
        }
        else
            Debug.LogError(InversiveUtilities.NotFoundMessage($"ExecuteAction() Failed : No sessions found"));
    }

    /// <summary>
    /// Starts a specific chapter of the experience.
    /// </summary>
    /// <param name="chapterName">Name of the chapter to be started.</param>
    /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
    /// <returns>An IEnumerator coroutine.</returns>
    public static IEnumerator StartChapter(string chapterName, Action<bool> callback)
    {
        var chapter = InversiveService.GetExperience().Chapters.Where(x => x.Name == chapterName).FirstOrDefault();
        ExperienceSession.CurrentChapter = chapter;
        ExperienceSession.CurrentChapterId = chapter.Id;
        ExperienceSession.LastUpdateDate = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(SessionId))
        {
            using (var request = UnityWebRequest.Get(GetUri($"Chapter?session={SessionId}&chapterId={chapter.Id}")))
            {
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(InversiveUtilities.SuccessMessage("StartChapter : Data send successfully !"));
                    callback(true);
                }
                else
                {
                    Debug.LogError(InversiveUtilities.CallFailedMessage($"StartChapter() Failed : {request.error}"));
                    callback(false);
                }
            }
        }
        else
        {
            Debug.LogError(InversiveUtilities.CallFailedMessage($"StartChapter() Failed : Session Id not found"));
            callback(false);
        }
    }

    /// <summary>
    /// Ends the session.
    /// Must be called to be able to call the GetGlobalScore() and GetDisplayedGlobalScore() functions.
    /// </summary>
    /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
    /// <returns>An IEnumerator coroutine.</returns>
    public static IEnumerator End(Action<bool> callback)
    {
        ExperienceSession.EndDate = DateTime.UtcNow;
        if (ExperienceSession.StartDate.HasValue)
            ExperienceSession.TimePlayed = DateTime.UtcNow - ExperienceSession.StartDate.Value;
        InversiveService.SetExperienceSession(ExperienceSession);
        if (!string.IsNullOrEmpty(SessionId))
        {
            using (var request = UnityWebRequest.Get(GetUri($"End?session={SessionId}")))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(InversiveUtilities.CallFailedMessage($"End() Failed : {request.error}"));
                    callback(false);
                }
                else
                {
                    Debug.Log(InversiveUtilities.SuccessMessage("Ended successfully !"));
                    ExperienceSessionSummaryModel summaryModel = JsonConvert.DeserializeObject<ExperienceSessionSummaryModel>(request.downloadHandler.text);
                    ExperienceSummary = summaryModel;
                    callback(true);
                }
            }
        }
        else
        {
            Debug.LogError(InversiveUtilities.CallFailedMessage($"End() Failed : Session Id not found"));
            callback(false);
        }

    }

    /// <summary>
    /// Resets the session by deleting its data from the current session.
    /// </summary>
    /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
    /// <returns>An IEnumerator coroutine.</returns>
    public static IEnumerator Retry(Action<bool> callback)
    {
        PlayerPrefs.DeleteKey("ExperienceSession");
        ExperienceSession = new ExperienceSessionModel { Id = SessionId };
        if (!string.IsNullOrEmpty(SessionId))
        {
            using (var request = UnityWebRequest.Get(GetUri($"Manage/retry?session={SessionId}")))
            {
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(InversiveUtilities.CallFailedMessage($"Retry() Failed : {request.error}"));
                    callback(false);
                }
                else
                {
                    Debug.Log(InversiveUtilities.SuccessMessage($"Retry Launched successfully !"));
                    ExperienceSession = JsonConvert.DeserializeObject<ExperienceSessionModel>(request.downloadHandler.text);
                    InversiveService.SetExperience(ExperienceSession.Experience);
                    callback(true);
                }
            }
        }
        else
        {
            Debug.LogError(InversiveUtilities.CallFailedMessage($"Retry() Failed : Session Id not found"));
            callback(false);
        }
    }

    /// <summary>
    /// Closes the session.
    /// Must be called instead of End() if the session is not to be terminated. 
    /// </summary>
    /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
    /// <returns>An IEnumerator coroutine.</returns>
    public static IEnumerator Close(Action<bool> callback)
    {
        ExperienceSession.LastUpdateDate = DateTime.UtcNow;
        InversiveService.SetExperienceSession(ExperienceSession);
        if (!string.IsNullOrEmpty(SessionId))
        {
            using (var request = UnityWebRequest.Get(GetUri($"Manage/close?session={SessionId}")))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(InversiveUtilities.CallFailedMessage($"Close() Failed : {request.error}"));
                    callback(false);
                }
                else
                {
                    Debug.Log(InversiveUtilities.SuccessMessage($"Closed successfully !"));
                    callback(true);
                }
            }
        }
        else
        {
            Debug.LogError(InversiveUtilities.CallFailedMessage($"Close() Failed : Session Id not found"));
            callback(false);
        }
    }


    /// <summary>
    /// Saves JSON data associated with the session.
    /// </summary>
    /// <param name="obj">Object to be serialized and saved as JSON.</param>
    /// <param name="callback">Callback function invoked with the saved JSON upon success or null upon failure.</param>
    /// <returns>An IEnumerator coroutine.</returns>
    public static IEnumerator SaveJson(object obj, Action<string> callback)
    {
        if (!string.IsNullOrEmpty(SessionId))
        {
            var json = JsonConvert.SerializeObject(obj);
            using (var request = UnityWebRequest.Put(GetUri($"Save?session={SessionId}"), json))
            {
                request.method = "POST";
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log(InversiveUtilities.SuccessMessage($"Save Json successfully !"));
                    ExperienceSession.Json = json;
                    callback(ExperienceSession.Json);
                }
                else
                {
                    Debug.LogError(InversiveUtilities.CallFailedMessage($"SaveJson() Failed : {request.error}"));
                    callback(null);
                }
            }
        }
        else
        {
            Debug.LogError(InversiveUtilities.CallFailedMessage($"SaveJson() Failed : Session Id not found"));
            callback(null);
        }
    }

    #endregion
}

