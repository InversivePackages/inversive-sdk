# Inversive SDK Troubleshooting

## Overview

Inversive SDK is a new tool, but encountering issues during usage is possible. Below are some common problems you might face and how to resolve them.

## Common Issues:

### `You need to have a token to access the editor experience`

   - To resolve this issue, open the "Generate App Id" window in Unity (`InversiveSDK/Generate App Id`). Fill in your Inversive credentials and click the "Generate App Id" button. Once done, you'll gain access to the `Experience model` in the experience editor window. 

### `Issue in Demo Scene`

   - If you encounter issues using the Demo Scene in Unity, you may need to import the json model (Inversive SDK/Samples/Demo/ExperienceModelToImport.json) by unfolding the Import Model section and loading the model as a json file.

### `You've pushed your Unity project changes to Git, but other collaborators don't have the latest updates on your experience model.`

   - The experience model values are stored in PlayerPrefs, hence not automatically synced.
   - To sync the model among collaborators, push the model changes to the remote experience model selecting `Push Model`. Collaborators can then update their local models by selecting `Update Model` in the experience editor.

### Error codes : 

#### `002: Success`

   - This code indicates a successful API call, relevant to methods: Init, StartExperience, ExecuteAction, StartChapter, End, Retry, Close, SaveJson.

#### `003: Not Found`

##### Methods

###### `ExecuteAction() Failed`

   - Session not found Or Session Id not found: Contact Inversive's support team for assistance.
   - No chapters found: Ensure your remote model contains chapters. If not, add chapters via the Experience Editor and update the remote model by clicking `Push Model`.
   - No chapters named {chapter name} found: Add the respective chapter in the Experience Editor or update the remote model.
   - No actions named {action name} exist in the {chapter name} chapter: Add the specific action to the mentioned chapter or update the remote model.

#### `005: Calling Api Failed`

##### Methods

###### `Init() Failed`

   - Retrieving the id of the session has failed. 
   - Session not found Or Session Id not found: Contact Inversive's support team for assistance.

###### `ExecuteAction() Failed`

   - Session not found Or Session Id not found: Contact Inversive's support team for assistance.
   - Action not present in the remote model; consider updating the remote model by selecting `Push Model` in the experience editor window.
   - Chapter absent in the remote model; consider updating the remote model by selecting `Push Model` in the experience editor window.
   - Parsing failure for sent value to match the correct action type (Boolean, Integer, String, or Float); consider modifying the action type for the concerned action in the experience editor window and update the remote model by clicking on `Push Model` in the experience editor window. You can also review the syntax of the sent value.

###### `StartChapter() Failed`

   - Session not found Or Session Id not found: Contact Inversive's support team for assistance.
   - The provided chapter name to initiate the chapter does not exist in the remote experience model; consider updating the remote model by selecting `Push Model` in the experience editor window.

###### `Start() Failed / End() Failed / Retry() Failed / Close() Failed / SaveJson() Failed`

   - Session not found Or Session Id not found: Contact Inversive's support team for assistance.

#### `007: Unprocessable Entity`

##### Methods
 
###### `ExecuteAction() Failed`

   - Failed to parse the sent value to the correct action type (Boolean, Integer, String, or Float). Modify the action type and update the remote model by clicking on `Push Model` in the experience editor window.

#### `011: Experience Model Validation failed `
   
   - Check the detailed error explanation to resolve Experience Model validation issues.

## Need Further Assistance ?

If you've encountered issues that remain unresolved or need additional help, feel free to reach out to our support team at [contact@inversive.fr](mailto:contact@inversive.fr). Our team is dedicated to assisting you in resolving any lingering issues or queries you may have regarding the Inversive SDK.