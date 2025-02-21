using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inversive.SDK
{
    public class InversiveSdk : MonoBehaviour
    {
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
            return InversiveExperience.GetScoringType();
        }

        /// <summary>
        /// Retrieves the model of the current chapter in the user's experience.
        /// </summary>
        /// <returns>The ExperienceChapterModel representing the current chapter.</returns>
        public static ExperienceChapterModel GetCurrentChapter()
        {
            return InversiveExperience.GetCurrentChapter();
        }

        /// <summary>
        /// Retrieves the model of the last action performed by the user.
        /// </summary>
        /// <returns>The ExperienceActionModel representing the last action.</returns>
        public static ExperienceActionModel GetLastAction()
        {
            return InversiveExperience.GetLastAction();
        }

        /// <summary>
        /// Recovers the overall score achieved in the experience.
        /// The global score is bounded between 0 and 100.
        /// Can be called after sending InversiveSdk.End().
        /// /// </summary>
        /// <returns>An int representing the global score.</returns>
        public static int GetGlobalScore()
        {
            return InversiveExperience.GetGlobalScore();
        }
        /// <summary>
        /// Retrieves the global score with the specified scoring applied.
        /// Can be called after sending InversiveSdk.End().
        /// </summary>
        /// <returns>
        /// If scoring type is 'Hundred', returns the GlobalScore as a string.
        /// If scoring type is 'Ten', returns the GlobalScore divided by 10 as a string.
        /// If scoring type is 'Twenty', returns the GlobalScore divided by 5 and rounded as a string.
        /// If scoring type is 'Letter', maps the GlobalScore to a corresponding letter grade (A, B, C, D, E, F).
        /// </returns>
        public static string GetDisplayedGlobalScore()
        {
            return InversiveExperience.GetDisplayedGlobalScore();
        }

        /// <summary>
        /// Retrieves the score of the last action performed by the user.
        /// </summary>
        /// <returns>The score of the last action as an integer.</returns>
        public static int GetLastActionScore()
        {
            return InversiveExperience.GetLastActionScore();
        }

        /// <summary>
        /// Retrieves the win score related to the user's experience.
        /// </summary>
        /// <returns>The win score as an integer.</returns>
        public static int GetWinScore()
        {
            return InversiveExperience.GetWinScore();
        }

        /// <summary>
        /// Retrieves a list of ExperienceChapterModel objects representing chapters in the user's experience.
        /// </summary>
        /// <returns>A list of ExperienceChapterModel objects.</returns>
        public static List<ExperienceChapterModel> GetChapters()
        {
            return InversiveExperience.GetChapters();
        }


        /// <summary>
        /// This method retrieves a JSON string that has been previously saved in the session.
        /// </summary>
        /// <returns>
        /// A string containing the JSON data saved in the session.
        /// </returns>
        public static string GetSavedJson()
        {
            return InversiveExperience.GetSavedJson();
        }


        #endregion

        #region Public Methods

        #region Sdk Deprecated Methods

        /// <summary>
        /// Initializes the user session.
        /// Init() must be called before using anything else.
        /// </summary>
        /// <param name="monobehaviour">The MonoBehaviour instance to start the coroutine.</param>
        /// <param name="callback">Callback function invoked with the session ID upon successful initialization.</param>
        [Obsolete("Init(MonoBehaviour monobehaviour ..) is deprecated. Use Init() without monobehaviour instead.")]
        public static void Init(MonoBehaviour monobehaviour, Action<string> callback)
        {
            monobehaviour.StartCoroutine(InversiveExperience.Init(x => callback(x)));
        }

        /// <summary>
        /// Starts the session by setting the start date.
        /// </summary>
        /// <param name="monobehaviour">The MonoBehaviour instance to start the coroutine.</param>
        /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
        [Obsolete("StartExperience(MonoBehaviour monobehaviour ..) is deprecated. Use StartExperience() without monobehaviour instead.")]
        public static void StartExperience(MonoBehaviour monobehaviour, Action<bool> callback)
        {
            monobehaviour.StartCoroutine(InversiveExperience.Start(x => callback(x)));
        }

        /// <summary>
        /// Executes a specific action within a chapter of the experience.
        /// Here's an example implementation with a chapter name "FirstChapter" and an action name "FirstAction", values a list of string values :
        /// InversiveSdk.ExecuteAction(this, "FirstChapter", "FirstAction" , values, (x) => { Debug.Log($"Action score : {x}"); });
        /// </summary>
        /// <param name="monobehaviour">The MonoBehaviour instance to start the coroutine.</param>
        /// <param name="chapterName">Name of the chapter containing the action.</param>
        /// <param name="actionName">Name of the action to be executed.</param>
        /// <param name="values">Values associated with the action. Sent as List of String</param>
        /// <param name="callback">Callback function invoked with the calculated score upon completion.</param>
        [Obsolete("ExecuteAction(MonoBehaviour monobehaviour ..) is deprecated. Use ExecuteAction() without monobehaviour instead.")]
        public static void ExecuteAction(MonoBehaviour monobehaviour, string chapterName, string actionName, List<string> values, Action<int?> callback)
        {
            monobehaviour.StartCoroutine(InversiveExperience.ExecuteAction(chapterName, actionName, values, x => callback(x)));
        }

        /// <summary>
        /// Starts a specific chapter of the experience.
        /// </summary>
        /// <param name="monobehaviour">The MonoBehaviour instance to start the coroutine.</param>
        /// <param name="chapterName">Name of the chapter to be started.</param>
        /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
        [Obsolete("StartChapter(MonoBehaviour monobehaviour ..) is deprecated. Use StartChapter() without monobehaviour instead.")]
        public static void StartChapter(MonoBehaviour monobehaviour, string chapterName, Action<bool> callback)
        {
            monobehaviour.StartCoroutine(InversiveExperience.StartChapter(chapterName, x => callback(x)));
        }

        /// <summary>
        /// Ends the session.
        /// Must be called to be able to call the GetGlobalScore() and GetDisplayedGlobalScore() functions.
        /// </summary>
        /// <param name="monobehaviour">The MonoBehaviour instance to start the coroutine.</param>
        /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
        [Obsolete("End(MonoBehaviour monobehaviour ..) is deprecated. Use End() without monobehaviour instead.")]
        public static void End(MonoBehaviour monobehaviour, Action<bool> callback)
        {
            monobehaviour.StartCoroutine(InversiveExperience.End(x => callback(x)));
        }

        /// <summary>
        /// Resets the session by deleting its data from the current session.
        /// </summary>
        /// <param name="monobehaviour">The MonoBehaviour instance to start the coroutine.</param>
        /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
        [Obsolete("Retry(MonoBehaviour monobehaviour ..) is deprecated. Use Retry() without monobehaviour instead.")]
        public static void Retry(MonoBehaviour monobehaviour, Action<bool> callback)
        {
            monobehaviour.StartCoroutine(InversiveExperience.Retry(x => callback(x)));
        }

        /// <summary>
        /// Closes the session.
        /// Must be called instead of End() if the session is not to be terminated. 
        /// </summary>
        /// <param name="monobehaviour">The MonoBehaviour instance to start the coroutine.</param>
        /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
        [Obsolete("Close(MonoBehaviour monobehaviour ..) is deprecated. Use Close() without monobehaviour instead.")]
        public static void Close(MonoBehaviour monobehaviour, Action<bool> callback)
        {
            monobehaviour.StartCoroutine(InversiveExperience.Close(x => callback(x)));
        }

        /// <summary>
        /// Saves JSON data associated with the session.
        /// </summary>
        /// <param name="monobehaviour">The MonoBehaviour instance to start the coroutine.</param>
        /// <param name="obj">Object to be serialized and saved as JSON.</param>
        /// <param name="callback">Callback function invoked with the saved JSON upon success or null upon failure.</param>
        [Obsolete("SaveJson(MonoBehaviour monobehaviour ..) is deprecated. Use SaveJson() without monobehaviour instead.")]
        public static void SaveJson(MonoBehaviour monobehaviour, object obj, Action<string> callback)
        {
            monobehaviour.StartCoroutine(InversiveExperience.SaveJson(obj, x => callback(x)));
        }

        #endregion

        #region New Sdk Methods

        /// <summary>
        /// Initializes the user session.
        /// Init() must be called before using anything else.
        /// </summary>
        /// <param name="callback">Callback function invoked with the session ID upon successful initialization.</param>
        public static void Init(Action<string> callback)
        {
            CoroutineRunner.Instance.StartCoroutine(InversiveExperience.Init(x => callback(x)));
        }

        /// <summary>
        /// Starts the session by setting the start date.
        /// </summary>
        /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
        [Obsolete("The logic is now handled directly within Init().")]
        public static void StartExperience(Action<bool> callback)
        {
            CoroutineRunner.Instance.StartCoroutine(InversiveExperience.Start(x => callback(x)));
        }

        /// <summary>
        /// Executes a specific action within a chapter of the experience.
        /// Here's an example implementation with a chapter name "FirstChapter" and an action name "FirstAction", values a list of string values :
        /// InversiveSdk.ExecuteAction(this, "FirstChapter", "FirstAction" , values, (x) => { Debug.Log($"Action score : {x}"); });
        /// </summary>
        /// <param name="chapterName">Name of the chapter containing the action.</param>
        /// <param name="actionName">Name of the action to be executed.</param>
        /// <param name="values">Values associated with the action. Sent as List of String</param>
        /// <param name="callback">Callback function invoked with the calculated score upon completion.</param>
        public static void ExecuteAction(string chapterName, string actionName, List<string> values, Action<int?> callback)
        {
            CoroutineRunner.Instance.StartCoroutine(InversiveExperience.ExecuteAction(chapterName, actionName, values, x => callback(x)));
        }

        /// <summary>
        /// Starts a specific chapter of the experience.
        /// </summary>
        /// <param name="chapterName">Name of the chapter to be started.</param>
        /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
        public static void StartChapter(string chapterName, Action<bool> callback)
        {
            CoroutineRunner.Instance.StartCoroutine(InversiveExperience.StartChapter(chapterName, x => callback(x)));
        }

        /// <summary>
        /// Ends the session.
        /// Must be called to be able to call the GetGlobalScore() and GetDisplayedGlobalScore() functions.
        /// </summary>
        /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
        public static void End(Action<bool> callback)
        {
            CoroutineRunner.Instance.StartCoroutine(InversiveExperience.End(x => callback(x)));
        }

        /// <summary>
        /// Resets the session by deleting its data from the current session.
        /// </summary>
        /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
        public static void Retry(Action<bool> callback)
        {
            CoroutineRunner.Instance.StartCoroutine(InversiveExperience.Retry(x => callback(x)));
        }

        /// <summary>
        /// Closes the session.
        /// Must be called instead of End() if the session is not to be terminated. 
        /// </summary>
        /// <param name="callback">Callback function invoked with a boolean indicating success or failure.</param>
        public static void Close(Action<bool> callback)
        {
            CoroutineRunner.Instance.StartCoroutine(InversiveExperience.Close(x => callback(x)));
        }

        /// <summary>
        /// Saves JSON data associated with the session.
        /// </summary>
        /// <param name="obj">Object to be serialized and saved as JSON.</param>
        /// <param name="callback">Callback function invoked with the saved JSON upon success or null upon failure.</param>
        public static void SaveJson(object obj, Action<string> callback)
        {
            CoroutineRunner.Instance.StartCoroutine(InversiveExperience.SaveJson(obj, x => callback(x)));
        }

        #endregion

        #endregion
    }

}
