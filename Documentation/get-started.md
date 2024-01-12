# Inversive SDK Get Started

## Prerequisites

>[!IMPORTANT]
>
>You must have follow the [Installation Guide](./InstallationGuide.md) and import the sample Inversive SDK Starter Assets.

### Set up your unity environnement

#### Setup

1. You must have **generated an app id** for your unity project. (see how [Generate an App Id](./generate-app-id.md#generate-app-id))


2. :
    - If you **have an experience model as json file** to import:
    In the experience editor, you must have **load** the json file:
    See how [Load sample json model](./retrieve-model.md#loading-a-model-via-a-json-file)

    - You are starting from scratch : 
    See how **edit** your experience model in [Experience Editor](./experience-editor.md)

3. In the experience editor, **after importing your model** , you must have **Push Model** to update the remote model stored by our platform. (see how [Push Model](./share-model.md#pushing-a-model))

#### Working with collaborators 

If you are a team of content producers, you can use the collaboration tools provided by the sdk to work on the same experience model: 
- See how in [Share Model](./share-model.md)

##### SDK Methods Implementation

Here are sample implementations of the Inversive SDK methods Init(), StartExperience(), ExecuteAction(), End() and Retry(). They are extracted from the c# script GameManager (Inversive SDK/Samples/Demo/Scripts/GameManager.cs)

###### `Init()`
- This method is called before anything else to retrieve the user session information. 
 here's an example (from Inversive SDK/Samples/Demo/Scripts/GameManager.cs):
    ```csharp
        private void Awake()
        {
            //...
            //Initialize your sdk & retrieve your session informations
            InversiveSdk.Init(this, (x) =>
            {
                Debug.Log($"Initialized & session Id : {x}");
            });
        }
    ```
###### `StartExperience()`
- This method is called to signal the start of the session by the user.
 here's an example (from Inversive SDK/Samples/Demo/Scripts/GameManager.cs):
    ```csharp
        //...
        //Signals that the session has been started
        InversiveSdk.StartExperience(this, (IsSuccess) =>
        {
            Debug.Log($"Started Experience succeed ? : {IsSuccess}");
        });
        //...
    ```

###### `ExecuteAction()`
- This method is called when an action in your experience has been made. 
 here's an example (from Inversive SDK/Samples/Demo/Scripts/GameManager.cs):
    ```csharp
            //...
            //Report that the action has been performed and retrieve the score for that action
            InversiveSdk.ExecuteAction(this, "FirstChapter", actionName, values, (score) =>
            {
                Debug.Log($"Action score : {score}");
            });

            //...
    ```
###### `End()`
- This method is called when the experience has been finished by the user and the session has ended.
 here's an example (from Inversive SDK/Samples/Demo/Scripts/GameManager.cs):
    ```csharp
      
            //...
            //Report session ended
            InversiveSdk.End(this, (IsSuccess) =>
            {
                Debug.Log($"Ended Experience succeed ? : {IsSuccess}");
                if (IsSuccess)
                {
                    GlobalScore.text = $"Score : {InversiveSdk.GetDisplayedGlobalScore()}";
                    Feedback.text = InversiveSdk.GetGlobalScore() >= InversiveSdk.GetWinScore() ? "you succeeded" : "you failed";
                }
            });
            //...
    ```


### Troubleshooting

Inversive SDK is a new tool, but encountering issues during usage is possible. You can go to the troubleshooting section where there are some common problems you might face and how to resolve them : [Troubleshooting](./troubleshooting.md)


