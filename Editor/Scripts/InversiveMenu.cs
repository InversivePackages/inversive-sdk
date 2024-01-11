using InversiveSdkEditor;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class InversiveMenu : EditorWindow
{
    [MenuItem("InversiveSDK/Home")]
    public static void Init()
    {
        EditorWindow window = GetWindow<InversiveMenu>("Home");
        window.minSize = new Vector2(650, 500);
        window.maxSize = new Vector2(650, 500);
        window.Show();
    }

    private static readonly string changeLogfullPath = "Packages/com.inversive.inversive-sdk/CHANGELOG.md";
    private static readonly string packagefullPath = "Packages/com.inversive.inversive-sdk/package.json";
    private static readonly string ChangeLogGUID = "";
    private static readonly string ResourcesGUID = "";
    private static readonly string BuiltInGUID = "";
    private static readonly string InversiveIconPath = "Packages/com.inversive.inversive-sdk/Editor/UI/inversive-logo.png";


    public static readonly string ChangelogURL = new Uri(InversiveUtilities.GetApiUrl() + "dev/changelog/get-latest/").AbsoluteUri;

    private static readonly string ManualURL = "https://github.com/InversivePackages/inversive-sdk/blob/main/Documentation/InstallationGuide.md";
    private static readonly string BasicURL = "https://github.com/InversivePackages/inversive-sdk/blob/main/Documentation/CodeDocumentation.md";
    private static readonly string BeginnerURL = "https://github.com/InversivePackages/inversive-sdk/blob/main/Documentation/GetStarted.md";
    private static readonly string SiteURL = "";
    private static readonly string StoreURL = "";

    private static readonly GUIContent SamplesTitle = new GUIContent("Inversive Samples", "Import samples using our SDK");
    private static readonly GUIContent ResourcesTitle = new GUIContent("Documentation", "Check the online wiki for various topics about how to use ASE with node examples and explanations");
    private static readonly GUIContent ManageTitle = new GUIContent("Actions", "");
    private static readonly GUIContent UpdateTitle = new GUIContent("Latest Update", "Check the lastest additions, improvements and bug fixes done to ASE");
    private static readonly GUIContent InversiveSDKEditorTitle = new GUIContent("Inversive SDK Editor", "Are you using the latest version? Now you know");

    Vector2 m_scrollPosition = Vector2.zero;
    Preferences.ShowOption m_startup = Preferences.ShowOption.Never;

    [NonSerialized]
    Texture packageIcon = null;
    [NonSerialized]
    Texture textIcon = null;
    [NonSerialized]
    Texture webIcon = null;
    [NonSerialized]
    Texture experienceIcon = null;

    GUIContent InversiveSamplesbutton = null;
    GUIContent Manualbutton = null;
    GUIContent Basicbutton = null;
    GUIContent Beginnerbutton = null;

    GUIContent LoginButton = null;
    GUIContent ExperienceEditorButton = null;

    GUIContent InversiveIcon = null;
    RenderTexture rt;

    [NonSerialized]
    GUIStyle m_buttonStyle = null;
    [NonSerialized]
    GUIStyle m_buttonLeftStyle = null;
    [NonSerialized]
    GUIStyle m_buttonRightStyle = null;
    [NonSerialized]
    GUIStyle m_minibuttonStyle = null;
    [NonSerialized]
    GUIStyle m_labelStyle = null;
    [NonSerialized]
    GUIStyle m_linkStyle = null;

    private ChangeLogInfoModel m_changeLog;
    private bool m_infoDownloaded = false;
    private string m_newVersion = string.Empty;

    private void OnEnable()
    {
        rt = new RenderTexture(16, 16, 0);
        rt.Create();

        m_startup = (Preferences.ShowOption)EditorPrefs.GetInt(Preferences.PrefStartUp, 0);

        if (textIcon == null)
        {
            Texture icon = EditorGUIUtility.IconContent("TextAsset Icon").image;
            var cache = RenderTexture.active;
            RenderTexture.active = rt;
            Graphics.Blit(icon, rt);
            RenderTexture.active = cache;
            textIcon = rt;

            Manualbutton = new GUIContent(" Installation Guide", textIcon);
            Basicbutton = new GUIContent(" Code Documentation", textIcon);
            Beginnerbutton = new GUIContent(" Getting started", textIcon);
        }

        if (packageIcon == null)
        {
            packageIcon = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image;
            InversiveSamplesbutton = new GUIContent(" Inversive Samples", packageIcon);
        }

        if (experienceIcon == null)
        {
            experienceIcon = EditorGUIUtility.IconContent("SettingsIcon").image;
            ExperienceEditorButton = new GUIContent(" Experience Editor", experienceIcon);
        }

        if (webIcon == null)
        {
            webIcon = EditorGUIUtility.IconContent("BuildSettings.Web.Small").image;
            LoginButton = new GUIContent(" Generate App Id", webIcon);
        }

        if (m_changeLog == null)
        {
            var changelog = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(ChangeLogGUID));
            string jsonContent = File.ReadAllText(packagefullPath);
            var packageModel = PackageModel.CreateFromJSON(jsonContent);
            string lastUpdate = string.Empty;
            if (changelog != null)
            {
                lastUpdate = changelog.text.Substring(0, changelog.text.IndexOf("\nv", 50));// + "\n...";
                lastUpdate = lastUpdate.Replace("    *", "    \u25CB");
                lastUpdate = lastUpdate.Replace("* ", "\u2022 ");
            }
            m_changeLog = new ChangeLogInfoModel(packageModel.version, lastUpdate);
        }

        if (InversiveIcon == null)
        {
            InversiveIcon = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(InversiveIconPath));
        }
    }

    private void OnDisable()
    {
        if (rt != null)
        {
            rt.Release();
            DestroyImmediate(rt);
        }
    }

    private void OnGUI()
    {
        if (!m_infoDownloaded)
        {
            m_infoDownloaded = true;
            //LoadChangelog();
            StartBackgroundTask(StartRequest(ChangelogURL, () =>
            {
                var temp = ChangeLogInfoModel.CreateFromJSON(www.downloadHandler.text);
                m_changeLog.latestChanges = temp?.latestChanges ?? "";
                m_newVersion = temp?.version ?? m_changeLog.version;
                Repaint();
            }));
        }

        if (m_buttonStyle == null)
        {
            m_buttonStyle = new GUIStyle(GUI.skin.button);
            m_buttonStyle.alignment = TextAnchor.MiddleLeft;
        }

        if (m_buttonLeftStyle == null)
        {
            m_buttonLeftStyle = new GUIStyle("ButtonLeft");
            m_buttonLeftStyle.alignment = TextAnchor.MiddleLeft;
            m_buttonLeftStyle.margin = m_buttonStyle.margin;
            m_buttonLeftStyle.margin.right = 0;
        }

        if (m_buttonRightStyle == null)
        {
            m_buttonRightStyle = new GUIStyle("ButtonRight");
            m_buttonRightStyle.alignment = TextAnchor.MiddleLeft;
            m_buttonRightStyle.margin = m_buttonStyle.margin;
            m_buttonRightStyle.margin.left = 0;
        }

        if (m_minibuttonStyle == null)
        {
            m_minibuttonStyle = new GUIStyle("MiniButton");
            m_minibuttonStyle.alignment = TextAnchor.MiddleLeft;
            m_minibuttonStyle.margin = m_buttonStyle.margin;
            m_minibuttonStyle.margin.left = 20;
            m_minibuttonStyle.normal.textColor = m_buttonStyle.normal.textColor;
            m_minibuttonStyle.hover.textColor = m_buttonStyle.hover.textColor;
        }

        if (m_labelStyle == null)
        {
            m_labelStyle = new GUIStyle("BoldLabel");
            m_labelStyle.margin = new RectOffset(4, 4, 4, 4);
            m_labelStyle.padding = new RectOffset(2, 2, 2, 2);
            m_labelStyle.fontSize = 13;
        }

        if (m_linkStyle == null)
        {
            var inv = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath("1004d06b4b28f5943abdf2313a22790a")); // find a better solution for transparent buttons
            m_linkStyle = new GUIStyle();
            m_linkStyle.normal.textColor = new Color(0.2980392f, 0.4901961f, 1f);
            m_linkStyle.hover.textColor = Color.white;
            m_linkStyle.active.textColor = Color.grey;
            m_linkStyle.margin.top = 3;
            m_linkStyle.margin.bottom = 2;
            m_linkStyle.hover.background = inv;
            m_linkStyle.active.background = inv;
        }

        EditorGUILayout.BeginHorizontal(GUIStyle.none, GUILayout.ExpandWidth(true));
        {
            // left column
            EditorGUILayout.BeginVertical(GUILayout.Width(175));
            {
                GUILayout.Label(SamplesTitle, m_labelStyle);
                if (GUILayout.Button(InversiveSamplesbutton, m_buttonStyle))
                    ImportSample(InversiveSamplesbutton.text, BuiltInGUID);

                GUILayout.Space(10);

                GUILayout.Label(ResourcesTitle, m_labelStyle);
                if (GUILayout.Button(Manualbutton, m_buttonStyle))
                    Application.OpenURL(ManualURL);

                if (GUILayout.Button(Beginnerbutton, m_buttonStyle))
                    Application.OpenURL(BeginnerURL);

                if (GUILayout.Button(Basicbutton, m_buttonStyle))
                    Application.OpenURL(BasicURL);
            }
            EditorGUILayout.EndVertical();

            // right column
            EditorGUILayout.BeginVertical(GUILayout.Width(650 - 175 - 9), GUILayout.ExpandHeight(true));
            {
                GUILayout.Label(ManageTitle, m_labelStyle);

                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    if (GUILayout.Button(ExperienceEditorButton, GUILayout.ExpandWidth(true)))
                    {
                        Close();
                        ExperienceEditor.ShowWindow();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    if (GUILayout.Button(LoginButton, GUILayout.ExpandWidth(true)))
                    {
                        InversiveLoginWindow.Init();
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(4);
                GUILayout.Label(UpdateTitle, m_labelStyle);
                m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, "ProgressBarBack", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                GUILayout.Label(m_changeLog.latestChanges, "WordWrappedMiniLabel", GUILayout.ExpandHeight(true));
                GUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    EditorGUILayout.BeginVertical();
                    GUILayout.Label(InversiveSDKEditorTitle, m_labelStyle);
                    GUILayout.Label("Installed Version: " + PackageModel.CreateFromJSON(File.ReadAllText(packagefullPath)).version);

                    if (CompareVersions(m_newVersion, PackageModel.CreateFromJSON(File.ReadAllText(packagefullPath)).version) > 0)
                    {
                        var cache = GUI.color;
                        GUI.color = Color.red;
                        GUILayout.Label("New version available: " + m_newVersion, "BoldLabel");
                        GUI.color = cache;
                    }
                    else
                    {
                        var cache = GUI.color;
                        GUI.color = Color.green;
                        GUILayout.Label("You are using the latest version", "BoldLabel");
                        GUI.color = cache;
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Download links:");
                    if (GUILayout.Button("Inversive", m_linkStyle))
                        Application.OpenURL(SiteURL);
                    GUILayout.Label("-");
                    if (GUILayout.Button("Github", m_linkStyle))
                        Application.OpenURL(StoreURL);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(7);
                    EditorGUILayout.EndVertical();

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginVertical();
                    GUILayout.Space(7);
                    GUILayout.Label(InversiveIcon, GUILayout.Height(100), GUILayout.Width(200));
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal("ProjectBrowserBottomBarBg", GUILayout.ExpandWidth(true), GUILayout.Height(22));
        {
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            var cache = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;
            m_startup = (Preferences.ShowOption)EditorGUILayout.EnumPopup("Show At Startup", m_startup, GUILayout.Width(220));
            EditorGUIUtility.labelWidth = cache;
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt(Preferences.PrefStartUp, (int)m_startup);
            }
        }
        EditorGUILayout.EndHorizontal();
        Repaint();
    }

    private void ImportSample(string pipeline, string guid)
    {
        if (EditorUtility.DisplayDialog("Import Sample", "This will import the samples for" + pipeline.Replace(" Samples", "") + ", please make sure the pipeline is properly installed and/or selected before importing the samples.\n\nContinue?", "Yes", "No"))
        {
            AssetDatabase.ImportPackage(AssetDatabase.GUIDToAssetPath(ResourcesGUID), false);
            AssetDatabase.ImportPackage(AssetDatabase.GUIDToAssetPath(guid), false);
        }
    }

    UnityWebRequest www;

    IEnumerator StartRequest(string url, Action success = null)
    {
        using (www = UnityWebRequest.Get(url))
        {
#if UNITY_2017_2_OR_NEWER
            yield return www.SendWebRequest();
#else
				yield return www.Send();
#endif

            while (www.isDone == false)
                yield return null;

            if (success != null)
                success();
        }
    }

    public static void StartBackgroundTask(IEnumerator update, Action end = null)
    {
        EditorApplication.CallbackFunction closureCallback = null;

        closureCallback = () =>
        {
            try
            {
                if (update.MoveNext() == false)
                {
                    if (end != null)
                        end();
                    EditorApplication.update -= closureCallback;
                }
            }
            catch (Exception ex)
            {
                if (end != null)
                    end();
                Debug.LogException(ex);
                EditorApplication.update -= closureCallback;
            }
        };

        EditorApplication.update += closureCallback;
    }

    public static int CompareVersions(string version1, string version2)
    {
        if (string.IsNullOrEmpty(version1))
        {
            return string.IsNullOrEmpty(version2) ? 0 : -1;
        }
        else if (string.IsNullOrEmpty(version2))
        {
            return 1;
        }

        string[] parts1 = version1.Split('.');
        string[] parts2 = version2.Split('.');

        int minLength = Math.Min(parts1.Length, parts2.Length);

        for (int i = 0; i < minLength; i++)
        {
            int part1 = int.Parse(parts1[i]);
            int part2 = int.Parse(parts2[i]);

            if (part1 != part2)
            {
                return part1 > part2 ? 1 : -1;
            }
        }

        return parts1.Length != parts2.Length ? parts1.Length > parts2.Length ? 1 : -1 : 0;
    }
}


[Serializable]
internal class ChangeLogInfoModel
{
    public int id;
    public string version;
    public string latestChanges;

    public static ChangeLogInfoModel CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<ChangeLogInfoModel>(jsonString);
    }

    public ChangeLogInfoModel(string version, string latestChanges)
    {
        this.version = version;
        this.latestChanges = latestChanges;
    }
}

[Serializable]
internal class PackageModel
{
    public string name;
    public string version;

    public static PackageModel CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PackageModel>(jsonString);
    }

}