# Inversive SDK Code Documentation

## Overview

Inversive SDK is supported on the following platforms:

- Windows
- WebGL
- Android

## Namespace: `Inversive.SDK`

### Class: `InversiveSdk`

### Properties

#### `GetScoringType()`
- Retrieves the scoring type of the experience.
- Possible scoring types:
    - Hundred: Represents scoring based on a hundred-point scale.
    - Ten: Represents scoring based on a ten-point scale.
    - Twenty: Represents scoring based on a twenty-point scale.
    - Letter: Represents scoring using letters (e.g., A, B, C).
- Returns: The scoring type as a `ScoringEnum`.

#### `GetCurrentChapter()`
- Retrieves the model of the current chapter in the user's experience.
- Returns: The `ExperienceChapterModel` representing the current chapter.

#### `GetLastAction()`
- Retrieves the model of the last action performed by the user.
- Returns: The `ExperienceActionModel` representing the last action.

#### `GetGlobalScore()`
- Recovers the overall score achieved in the experience:
  - The global score is bounded between 0 and 100.
- Returns: An int representing the global score.

#### `GetDisplayedGlobalScore()`
- Retrieves the global score with the specified scoring applied.
- Returns:
    - If scoring type is 'Hundred', returns the GlobalScore as a string.
    - If scoring type is 'Ten', returns the GlobalScore divided by 10 as a string.
    - If scoring type is 'Twenty', returns the GlobalScore divided by 5 and rounded as a string.
    - If scoring type is 'Letter', maps the GlobalScore to a corresponding letter grade (A, B, C, D, E, F).

#### `GetLastActionScore()`
- Retrieves the score of the last action performed by the user.
- Returns: The score of the last action as an integer.

#### `GetWinScore()`
- Retrieves the win score related to the user's experience.
- Returns: The win score as an integer.

#### `GetChapters()`
- Retrieves a list of `ExperienceChapterModel` objects representing chapters in the user's experience.
- Returns: A list of `ExperienceChapterModel` objects.

### Methods

#### `Init(MonoBehaviour monobehaviour, Action<string> callback)`
- Initializes the user session.
- Init() must be called before using anything else.
- Parameters:
    - `monobehaviour`: The MonoBehaviour instance to start the coroutine.
    - `callback`: Callback function invoked with the session ID upon successful initialization.

#### `StartExperience(MonoBehaviour monobehaviour, Action<bool> callback)`
- Starts the session by setting the start date.
- Parameters:
    - `monobehaviour`: The MonoBehaviour instance to start the coroutine.
    - `callback`: Callback function invoked with a boolean indicating success or failure.

#### `ExecuteAction(MonoBehaviour monobehaviour, string chapterName, string actionName, List<string> values, Action<int?> callback)`
- Executes a specific action within a chapter of the experience.
- Parameters:
    - `monobehaviour`: The MonoBehaviour instance to start the coroutine.
    - `chapterName`: Name of the chapter containing the action.
    - `actionName`: Name of the action to be executed.
    - `values`: Values associated with the action. Sent as List of String.
    - `callback`: Callback function invoked with the calculated score upon completion.

    Example:
    ```csharp
    InversiveSdk.ExecuteAction(this, "FirstChapter", "FirstAction" , values, (x) => { Debug.Log($"Action score : {x}"); });
    ```

#### `StartChapter(MonoBehaviour monobehaviour, string chapterName, Action<bool> callback)`
- Starts a specific chapter of the experience.
- Parameters:
    - `monobehaviour`: The MonoBehaviour instance to start the coroutine.
    - `chapterName`: Name of the chapter to be started.
    - `callback`: Callback function invoked with a boolean indicating success or failure.

#### `End(MonoBehaviour monobehaviour, Action<bool> callback)`
- Ends the session.
- Must be called to be able to call the `GetGlobalScore()` and `GetDisplayedGlobalScore()` functions.
- Parameters:
    - `monobehaviour`: The MonoBehaviour instance to start the coroutine.
    - `callback`: Callback function invoked with a boolean indicating success or failure.

    Example:
    ```csharp
    InversiveSdk.End(this, (IsSuccess) =>
    {
        if(IsSuccess){
            Debug.Log($"Global score : {InversiveSdk.GetDisplayedGlobalScore()}");
            Debug.Log($"Experience succeeded : {InversiveSdk.GetGlobalScore() >= InversiveSdk.GetWinScore()}");
        }
    });
    ```

#### `Retry(MonoBehaviour monobehaviour, Action<bool> callback)`
- Resets the session by deleting its data from the current session.
- Parameters:
    - `monobehaviour`: The MonoBehaviour instance to start the coroutine.
    - `callback`: Callback function invoked with a boolean indicating success or failure.

#### `Close(MonoBehaviour monobehaviour, Action<bool> callback)`
- Closes the session.
- Must be called instead of End() if the session is not to be terminated.
- Parameters:
    - `monobehaviour`: The MonoBehaviour instance to start the coroutine.
    - `callback`: Callback function invoked with a boolean indicating success or failure.

#### `SaveJson(MonoBehaviour monobehaviour, object obj, Action<string> callback)`
 - Saves JSON data associated with the session.
 - Parameters:
    - `monobehaviour`: The MonoBehaviour instance to start the coroutine.
    - `obj`: Object to be serialized and saved as JSON.
    - `callback`: Callback function invoked with the saved JSON upon success or null upon failure.