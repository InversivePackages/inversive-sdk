# Share a model

>[!IMPORTANT]
>
>Mandatory step to use Inversive Sdk.

## Overview

One of the main features of Inversive Sdk is the ability to **share a model** among developers. This is an important feature for setting up the sdk in unity. 

## How ?
--- 

### Pushing a model

Once the app ID has been generated, you can set up a chaptering for your Unity project (see [Experience Editor](./experience-editor.md)). Once this has been completed, you can click on the **Push Model** button to associate it with the app ID.

![Push Model](./images/push-model.png "Push Model"){ style="display: block; margin: 0 auto" }

Once done, a window should appear to confirm that your model has been saved on our platform. 

![Push Model Succeeded](./images/pushed-successfully.png "Push Model Succeeded"){ style="display: block; margin: 0 auto" }

>[!NOTE]
>
>Other developers can then load the model using the app ID of this experience (see [Loading a model via app ID](./retrieve-model.md)).

### Export a model (Json)

To share your model with other developers, you can extract it into a json file. To do this, simply click on the **Export Model** button. 

>[!WARNING]
>
>If you have already exported a model, it will be replaced by the current model.

![Export Model](./images/export-model.png "Export Model"){ style="display: block; margin: 0 auto" }

You'll find your model in your project's **"Assets/Inversive SDK"** folder. Other developers will be able to load the model via this file (see [Loading a model via a json file](./retrieve-model.md)).

![Export Model Succeeded](./images/exported-successfully.png "Export Model Succeeded"){ style="display: block; margin: 0 auto" }

>[!NOTE]
>
>Other developers can then load the model using json file loading. (see [Loading a model via a json file](./retrieve-model.md)).
