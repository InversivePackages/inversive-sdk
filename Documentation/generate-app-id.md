# Generate App Id

>[!IMPORTANT]
>
>Mandatory step to use Inversive Sdk.

## Overview
--- 

This is a **mandatory step** for setting up the sdk in unity. 

When you open the experience editor, you should see "You need to have an app id to access the editor experience". To access the experience model editor, you need to generate an app id. 

![Experience editor without app id](./images/experience-editor-without-appid.png "Experience editor without app id"){ style="display: block; margin: 0 auto" }

## How ?
--- 

>[!IMPORTANT]
>
>To generate an app ID, you must have an active account on the INVERSIVE platform (administrator or producer account).

To generate an app Id, click on **Generate App ID** from the InversiveSDK menu in your toolbar or from the main INVERSIVE SDK window. 

![Inversive Sdk Toolbar](./images/inversivesdk-toolbar.png "Inversive Sdk Toolbar"){ style="display: block; margin: 0 auto" }

A login window should appear. Please enter your inversive platform user email and password.

![Login](./images/login.png "Login"){ style="display: block; margin: 0 auto" }

After generating the app id by logging in, you should see the experience model editor.

The generated **app Id** is associated with the **experience model**.

![Experience editor with app id](./images/experience-editor-with-appid.png "Experience editor with app id"){ style="display: block; margin: 0 auto" }

>[!IMPORTANT]
>
>If you have several user profiles (producer or administrator) available on the platform, it is mandatory  that you create/publish your experience with the user profile that generated the app ID. This app ID is associated with the creation of an experience on an account!

## Why generate an app Id ?
---
Generating an app Id allows you to :

- Link the chaptering of an experience (*experience model*) to an experience creation form on the platform. On your Unity project, once an app Id has been generated with a profile, the chaptering set up on the project is associated with the creation of an experience with this profile, on the platform.

- Share the experience model with other developers of your team. The app Id generated can be transmitted to several developers (with or without an account on the INVERSIVE platform) so that they can modify or exploit the chaptering set up for this project.

- Facilitate the creation of an experience on the platform. In the experience creation form, once an app Id has been generated, the experience creation form is automatically filled in on the account with which I generated the app Id.
