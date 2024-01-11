# Installation Guide

## Package: Inversive SDK

### Steps to Install:

1. **Open Unity Editor**

2. **Access Package Manager:**

   - Navigate to **Window** > **Package Manager**.

3. **Add Package from Git URL:**

   - Click on the **"+"** icon in the top left corner of the Package Manager.
   
   - Select **"Add package from git URL..."** from the dropdown menu.

4. **Enter Git URL:**

   - Copy and paste the following Git URL: [Inversive SDK Git URL](https://github.com/InversivePackages/inversive-sdk.git)

5. **Confirm Installation:**

   - Click **"Add"** to begin the installation process.
   
6. **Using the Package:**

   - Refer to the package documentation or repository for instructions on utilizing the features and functionalities provided by the Inversive SDK.

7. **Import Samples**

   - Expand the "Samples" section and import the "Inversive SDK Starter Assets" by clicking on "Import." Afterward, you should see a folder named "Inversive SDK" within your "Samples" directory under the "Assets" folder.

8. **Home window**

   - Once the SDK has been installed on your Unity project, you will have access to its interface via the InversiveSDK menu on your toolbar.
   From this menu, you can access the SDK's main window  main window ("Home"), go directly to the chapter editor for your experience ("Experience Editor") or generate an App ID.

      - The main window ("Home") provides access to the documentation, the SDK's main features and the latest updates.
      - The Experience Editor displays the main interface for configuring the sequences of your experience as well as the type of actions you wish to collect and the scores associated with these actions. ( see [Experience Editor](./experience-editor.md))
      - Generating an App ID allows you to associate your Unity project with the experience that will be created on the INVERSIVE platform. ( see [Generate an App Id](./generate-app-id.md))

   
9. **Enjoy Exploring!**

   - Start integrating the Inversive SDK into your Unity projects and explore its capabilities.

### Unity Documentation : Through [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

Unity's own Package Manager supports [importing packages through a URL to a Git repo](https://docs.unity3d.com/Manual/upm-ui-giturl.html):

1. First, on this repository page, click the "Clone or download" button, and copy over this repository's HTTPS URL.  
2. Then click on the + button on the upper-left-hand corner of the Package Manager, select "Add package from git URL..." on the context menu, then paste this repo's URL!

While easier to follow than the first method, this one does not support dependency resolution and package upgrading when a new version is released.  So proceed with caution.
