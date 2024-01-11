# Inversive SDK Code Documentation

## Overview

The application provides a lot of features. They are documented below.

### Terminology  

#### Chapters

- In the Inversive SDK, a **Chapter** refers to a distinct segment or section within an experience. [ExperienceChapterModel](../Runtime/InversiveClasses.cs) represents a chapter within the experience. A chapter contains a list of actions. The **Name** property must be unique. (See [Chapter](./experience-editor.md#chapter)) 

#### Actions

- In the Inversive SDK, **Actions** are the individual tasks or actions that occur within a specific **chapter** of the experience. [ExperienceActionModel](../Runtime/InversiveClasses.cs) represents an **action** in a chapter. The **Name** property must be unique. (See [Action](./experience-editor.md#action))

>[!IMPORTANT]
>
>It is important to note that the names of both chapters and actions must be unique. So that they can be used as identifiers in the functions below. (See ExecuteAction() Or StartChapter() in the [Methods](#methods))

>[!NOTE]
>
>See [InversiveClasses](../Runtime/InversiveClasses.cs) for more information on these classes.

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
    - If scoring type is 'Letter', maps the GlobalScore to a corresponding letter grade (A, B, C ...).

#### `GetLastActionScore()`
- Retrieves the score of the last action performed by the user.
- Returns: The score of the last action as an integer.

#### `GetWinScore()`
- Retrieves the win score related to the user's experience.
- Returns: The win score as an integer.

#### `GetChapters()`
- Retrieves a list of **chapters** objects representing chapters in the user's experience.
- Returns: A list of `ExperienceChapterModel` objects. (See [InversiveClasses](../Runtime/InversiveClasses.cs))

#### `GetSavedJson()`
- This method retrieves a JSON string that has been previously saved in the session.
- Returns: A string containing the JSON data saved in the session. (See [InversiveClasses](../Runtime/InversiveClasses.cs))

### Methods

>[!IMPORTANT]
>
>The methods below call the Inversive platform sdk api. Here's the [swagger link](https://sdk.vrcxp.com/) for your implementations. 

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
- ⚠️ **Important**: This method uses the property **Name** of `ExperienceActionModel`and the property **Name** of `ExperienceChapterModel` as identifiers to retrieve the action concerned.
- Parameters:
    - `monobehaviour`: The MonoBehaviour instance to start the coroutine.
    - `chapterName`: Name of the chapter containing the action.
    - `actionName`: Name of the action to be executed.
    - `values`: Values associated with the action. Sent as List of String that can contain a single value **or** several values sent for an action whose response type (ActionResponseType property in [ExperienceActionModel](../Runtime/InversiveClasses.cs)) is of type MultipleValues.
    - `callback`: Callback function invoked with the calculated score upon completion.

    Example:
    ```csharp
    InversiveSdk.ExecuteAction(this, "FirstChapter", "FirstAction" , values, (x) => { Debug.Log($"Action score : {x}"); });
    ```

#### `StartChapter(MonoBehaviour monobehaviour, string chapterName, Action<bool> callback)`
- Starts a specific chapter of the experience.
- ⚠️ **Important** : This method uses the property **Name** of `ExperienceChapterModel` as identifier to retrieve the chapter concerned.
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
- The json is stored in a string property in the model representing the session. 
- ⚠️ **Warning** , Each time you save a json, the previous one is overwritten. 
- You can access the data via the [GetSavedJson()](#getsavedjson) method. 
- Parameters:
    - `monobehaviour`: The MonoBehaviour instance to start the coroutine.
    - `obj`: Object to be serialized and saved as JSON.
    - `callback`: Callback function invoked with the saved JSON upon success or null upon failure.
    
    Example:
    ```csharp
    public class User { public string FirstName; public string LastName; }

    User user = new User { FirstName = "Miyamoto", LastName = "Musashi" };

    InversiveSdk.SaveJson(this, user, (x) =>
    {
        Debug.Log($" json : {x}");
    });
    ```

### Troubleshooting

Inversive SDK is a new tool, but encountering issues during usage is possible. You can go to the troubleshooting section where there are some common problems you might face and how to resolve them : [Troubleshooting](./Troubleshooting.md)