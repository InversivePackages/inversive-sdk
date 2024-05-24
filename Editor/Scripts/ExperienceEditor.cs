using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class ExperienceEditor : EditorWindow
{
    [SerializeField]
    public ExperienceModel Experience = new ExperienceModel();

    private string AccessToken
    {
        get { return InversiveService.GetAccessToken(); }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                InversiveService.SetAccessToken(value);
                Repaint();
            }
        }
    }

    private GUIStyle redLabelStyle;

    GUIContent InversiveIcon = null;
    Vector2 m_scrollPosition = Vector2.zero;

    [NonSerialized]
    Texture plusIcon = null;
    [NonSerialized]
    Texture deleteBadgeIcon = null;
    [NonSerialized]
    Texture editIcon = null;
    [NonSerialized]
    Texture resetIcon = null;
    [NonSerialized]
    Texture rollbackIcon = null;
    [NonSerialized]
    Texture crossIcon = null;
    [NonSerialized]
    Texture webIcon = null;
    [NonSerialized]
    Texture textIcon = null;
    [NonSerialized]
    Texture generateAccessTokenIcon = null;

    RenderTexture rt;

    GUIContent AddChapterButton = null;
    GUIContent AddActionButton = null;
    GUIContent AddValueButton = null;
    GUIContent AddRatingLevelButton = null;
    GUIContent LoginButton = null;
    GUIContent RemoveAllButton = null;
    GUIContent SaveButton = null;
    GUIContent ResetButton = null;
    GUIContent RollbackButton = null;
    GUIContent DeleteButton = null;
    GUIContent UpdateButton = null;
    GUIContent ExportButton = null;

    private bool isFoldingChapters = false;

    private string UniqueExplanation =
    "For this type of response, please provide the expected response to the action.\nThe score value for the wrong answer can be changed, default value is 0.";

    private string IntervalExplanation =
    "For this type of answer, please provide:\n\n" +
    "- Expected value (corresponding to a score of 100): " +
    "This is the value you anticipate or consider average for a perfect score of 100.\n\n" +

    "- Minimum value (corresponding to a score of 0): " +
    "This is the lowest acceptable value, reflecting the lowest score of 0.\n\n" +

    "- Maximum value (corresponding to a score of 0): " +
    "This is the highest acceptable value, aligned with the score of 0.\n\n" +

    "The value you're aiming for falls somewhere within this range, " +
    "calculated based on the distances between these values. " +
    "This process helps determine where a particular score falls within " +
    "the spectrum between the minimum and maximum values.";

    private string MultipleValuesExplanation = "For this type of answer, you can enter multiple values along with their associated scores.\n\n" +
    "Each value should be accompanied by its respective score.\n\n" +
    "This allows for assigning scores to multiple items or data points, " +
    "enabling comprehensive evaluation across various elements.";

    private string RatingLevelExplanation = "For this type of answer, you can enter multiple rating levels along with their lower and upper limits, " +
    "and an associated score if matched.\n\n" +
    "Each rating level requires a specified lower and upper limit, " +
    "and if the provided value falls within these limits, an associated score will be assigned.\n\n" +
    "This allows for evaluating different ranges and assigning scores based on where a value falls within these limits.";

    private string ScoringTypeExplanation = "When you select a scoring type, two scores will be displayed based on your selection:\n" +
            "- The win score represents the score needed to succeed in the experience, ranging from 0 to 100.\n" +
            "- The global score represents your final score at the end of the experience, also ranging from 0 to 100.\n\n" +
            "Choose the scoring type:\n" +
            "- Hundred: Score ranges from 0 to 100.\n" +
            "- Ten: Score ranges from 0 to 10.\n" +
            "- Twenty: Score ranges from 0 to 20.\n" +
            "- Letter: Score ranges from A to F, where A represents the best score.";

    private string ScoreExplanation = "Score must be between 0 and 100.";

    private static readonly string CrossIconPath = "Packages/com.inversive.inversive-sdk/Editor/UI/cross-icon.png";
    private static readonly string InversiveIconPath = "Packages/com.inversive.inversive-sdk/Editor/UI/inversive-logo.png";

    private string ImportJsonAccessToken = string.Empty;
    private string jsonFilePath = string.Empty;
    private bool importModelFoldout = false;

    [MenuItem("InversiveSDK/Experience Editor", false, 1999)]
    public static void ShowWindow()
    {
        ExperienceEditor window = GetWindow<ExperienceEditor>("Experience Editor");
        window.minSize = new Vector2(550, 850);
        window.maxSize = new Vector2(550, 850);
        window.LoadData(); // Load existing data from your SDK or storage
        window.Show();
    }

    public static void ShowWindowAfterLogin()
    {
        ExperienceEditor window = GetWindow<ExperienceEditor>("Experience Editor");
        window.minSize = new Vector2(550, 850);
        window.maxSize = new Vector2(550, 850);
        window.LoadData(true); // Load existing data from your SDK or storage
        window.Show();
    }

    private void OnDestroy()
    {
        var json = JsonConvert.SerializeObject(Experience);
        PlayerPrefs.SetString("ExperienceLocal", json);
    }

    private void OnEnable()
    {
        rt = new RenderTexture(16, 16, 0);
        rt.Create();

        importModelFoldout = false;

        if (redLabelStyle == null)
        {
            redLabelStyle = new GUIStyle(EditorStyles.label);
            redLabelStyle.normal.textColor = Color.red;
        }

        if (textIcon == null)
        {
            Texture icon = EditorGUIUtility.IconContent("TextAsset Icon").image;
            var cache = RenderTexture.active;
            RenderTexture.active = rt;
            Graphics.Blit(icon, rt);
            RenderTexture.active = cache;
            textIcon = rt;

            ExportButton = new GUIContent(" Export Model", textIcon, "Export your model as json file in your Assets folder");
        }

        if (InversiveIcon == null)
        {
            InversiveIcon = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(InversiveIconPath));
        }

        if (crossIcon == null)
        {
            crossIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(CrossIconPath);
            DeleteButton = new GUIContent(crossIcon);
        }

        if (plusIcon == null)
        {
            plusIcon = EditorGUIUtility.IconContent("Toolbar Plus").image;
            AddChapterButton = new GUIContent(" Add Chapter", plusIcon);
            AddActionButton = new GUIContent(" Add Action", plusIcon);
            AddRatingLevelButton = new GUIContent(" Add Rating Level", plusIcon);
            AddValueButton = new GUIContent(" Add Value", plusIcon);
        }

        if (deleteBadgeIcon == null)
        {
            deleteBadgeIcon = EditorGUIUtility.IconContent("Toolbar Minus").image;
            RemoveAllButton = new GUIContent(" Remove All", deleteBadgeIcon);
        }

        if (editIcon == null)
        {
            editIcon = EditorGUIUtility.IconContent("CollabPush").image;
            SaveButton = new GUIContent(" Push Model", editIcon, "Push your model on remote");
        }

        if (resetIcon == null)
        {
            resetIcon = EditorGUIUtility.IconContent("d_Refresh").image;
            ResetButton = new GUIContent(" Reset", resetIcon, "Reset all values in your model");
        }

        if (rollbackIcon == null)
        {
            rollbackIcon = EditorGUIUtility.IconContent("d_CacheServerDisconnected").image;
            RollbackButton = new GUIContent(" Cancel Changes", rollbackIcon, "Cancels any changes you have made");
        }

        if (webIcon == null)
        {
            webIcon = EditorGUIUtility.IconContent("CollabPull").image;
            UpdateButton = new GUIContent(" Update Model", webIcon, "Pull the remote model");
        }
        if (generateAccessTokenIcon == null)
        {
            generateAccessTokenIcon = EditorGUIUtility.IconContent("BuildSettings.Web.Small").image;
            LoginButton = new GUIContent(" Generate App Id", generateAccessTokenIcon);
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(8);
        if (GUILayout.Button("Back to Manage Menu"))
        {
            Close();
            InversiveMenu.Init();
        }
        GUILayout.Space(4);
        GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
        importModelFoldout = InspectorCommon.DrawSectionHeader($"Import Model", InspectorCommon.HeaderType.Level1, importModelFoldout, true);
        if (importModelFoldout)
        {
            GUILayout.Space(4);
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("By App Id", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            {
                ImportJsonAccessToken = EditorGUILayout.TextField(ImportJsonAccessToken);
                if (GUILayout.Button("Import Model"))
                {
                    InversiveService.SetAccessToken(ImportJsonAccessToken);
                    EditorCoroutineUtility.StartCoroutine(GetExperienceHead(), this);
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(8);
            GUILayout.Label("By Json File", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            {
                jsonFilePath = GUILayout.TextField(jsonFilePath, GUILayout.Width(215));
                if (GUILayout.Button("Browse"))
                {
                    jsonFilePath = EditorUtility.OpenFilePanel("Load JSON File", "", "json");
                }
                if (GUILayout.Button("Load Model"))
                {
                    if (!string.IsNullOrEmpty(jsonFilePath) && File.Exists(jsonFilePath))
                    {
                        LoadExperienceModel(jsonFilePath);
                    }
                    else
                    {
                        Debug.LogError("Please provide a valid JSON file path.");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUILayout.Space(8);
        }
        GUILayout.EndVertical();
        GUILayout.Space(8);
        if (!string.IsNullOrEmpty(InversiveService.GetAccessToken()))
        {
            GUILayout.Label("App Id", EditorStyles.boldLabel);
            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            {
                AccessToken = EditorGUILayout.TextField(AccessToken);
                if (GUILayout.Button("Copy"))
                {
                    EditorGUIUtility.systemCopyBuffer = AccessToken;
                    Debug.Log("String copied to clipboard: " + AccessToken);
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(8);
            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(8);
            GUILayout.Label("Experience Model", EditorStyles.boldLabel);
            GUILayout.Space(8);
            if (GUILayout.Button(UpdateButton))
            {
                if (EditorUtility.DisplayDialog("Update Model", "Are you sure you want to update your model ?\n\n" + "You will lose all your advances.", "Continue", "Cancel", DialogOptOutDecisionType.ForThisSession, "ExperienceEditor.UndoChanges"))
                {
                    UpdateModel();
                }
            }
            if (InversiveService.GetExperience() != null && Experience != InversiveService.GetExperience())
            {
                if (GUILayout.Button(RollbackButton))
                {
                    if (EditorUtility.DisplayDialog("Undo changes", "Are you sure you want to undo your changes ?\n\n" + "You will lose all your advances made since the beginning of the session.", "Continue", "Cancel", DialogOptOutDecisionType.ForThisSession, "ExperienceEditor.UndoChanges"))
                    {
                        CancelChanges();
                    }
                }
            }
            if (GUILayout.Button(ResetButton))
            {
                if (EditorUtility.DisplayDialog("Reset model", "Are you sure you want to reset the model ? ", "Continue", "Cancel", DialogOptOutDecisionType.ForThisSession, "ExperienceEditor.Reset"))
                {
                    ResetModel();
                }
            }
            GUILayout.Space(16);
            EditorGUILayout.BeginHorizontal();
            GUIContent helpIcon_8 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
            helpIcon_8.tooltip = ScoringTypeExplanation;
            GUIStyle iconStyle_1 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            Rect rect_1 = GUILayoutUtility.GetRect(helpIcon_8, iconStyle_1);
            rect_1.width = EditorGUIUtility.labelWidth;
            GUI.Label(rect_1, helpIcon_8, iconStyle_1);
            EditorGUILayout.LabelField("Scoring Type:", GUILayout.Width(EditorGUIUtility.labelWidth));
            Experience.ScoringType = (ScoringEnum)EditorGUILayout.Popup((int)Experience.ScoringType, ScoringEnumDescriptions);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUIContent helpIcon_6 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
            helpIcon_6.tooltip = "The win score represents the score needed to succeed in the experience, ranging from 0 to 100.";
            GUIStyle iconStyle_5 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            Rect rect_5 = GUILayoutUtility.GetRect(helpIcon_6, iconStyle_5);
            rect_5.width = EditorGUIUtility.labelWidth;
            GUI.Label(rect_5, helpIcon_6, iconStyle_5);
            EditorGUILayout.LabelField("Win Score (0 to 100):", GUILayout.Width(EditorGUIUtility.labelWidth));
            Experience.WinScore = Mathf.Clamp(Experience.WinScore, 0, 100);
            Experience.WinScore = EditorGUILayout.IntField(Experience.WinScore);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.Label("Edit your chapters and actions", EditorStyles.boldLabel);
            GUILayout.Space(15);
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            isFoldingChapters = InspectorCommon.DrawSectionHeader("Chapters:", InspectorCommon.HeaderType.Level1, isFoldingChapters, true, $"{Experience.Chapters.Count}");
            if (isFoldingChapters)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                //Logic here
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(AddChapterButton))
                {
                    AddChapter();
                }
                if (GUILayout.Button(RemoveAllButton))
                {
                    var isOkRemoveAllChapters = EditorUtility.DisplayDialog("Remove All Chapters", "Do you really want to remove all chapters ?", "Yes", "No");
                    if (isOkRemoveAllChapters)
                    {
                        RemoveAllChapter();
                    }
                }
                EditorGUILayout.EndHorizontal();
                m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, "ProgressBarBack", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                DisplayChapters();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                InspectorCommon.DrawSeparationSpacing();
            }
            GUILayout.EndVertical();
            GUILayout.Space(8);
            if (GUILayout.Button(ExportButton))
            {
                if (InversiveUtilities.ValidateModel(Experience))
                {
                    ExportModel();
                }
            }
            if (GUILayout.Button(SaveButton))
            {
                if (InversiveUtilities.ValidateModel(Experience))
                {
                    if (EditorUtility.DisplayDialog("Push model", "Are you sure you want to push this model ?\n\n" + "This will be the model of reference on the remote experience model and so for the other developers", "Continue", "Cancel", DialogOptOutDecisionType.ForThisSession, "ExperienceEditor.Push"))
                    {
                        SaveDataToApi();
                    }
                }
            }
            GUILayout.Space(8);
            GUILayout.EndVertical();
            GUILayout.Space(8);
            // Center Vertically
            GUILayout.FlexibleSpace();
            // Horizontally centered
            using (var l = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(InversiveIcon, GUILayout.Height(100), GUILayout.Width(200));
                GUILayout.FlexibleSpace();
            }
        }
        else
        {
            GUILayout.Label("You need to have an app id to access the editor experience", EditorStyles.boldLabel);
            if (GUILayout.Button(LoginButton, GUILayout.ExpandWidth(true)))
            {
                Close();
                InversiveLoginWindow.Init();
            }
        }
    }

    private void DisplayChapters()
    {
        EditorGUILayout.BeginVertical("box");
        for (int i = 0; i < Experience.Chapters.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.BeginVertical(EditorStyles.selectionRect, new GUILayoutOption[0]);
            Experience.Chapters[i].isFoldout = InspectorCommon.DrawSectionHeader($"{Experience.Chapters[i].Name}", InspectorCommon.HeaderType.Level2, Experience.Chapters[i].isFoldout, true, $"{Experience.Chapters[i].Actions.Count} Actions");
            if (Experience.Chapters[i].isFoldout)
            {
                GUILayout.Space(8);
                EditorGUILayout.BeginHorizontal();
                GUIContent helpIcon_4 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                helpIcon_4.tooltip = "This is the name of the action.";
                GUIStyle iconStyle_4 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                Rect rect_4 = GUILayoutUtility.GetRect(helpIcon_4, iconStyle_4);
                rect_4.width = EditorGUIUtility.labelWidth;
                GUI.Label(rect_4, helpIcon_4, iconStyle_4);
                EditorGUILayout.LabelField("Name:", GUILayout.Width(EditorGUIUtility.labelWidth));
                Experience.Chapters[i].Name = EditorGUILayout.TextField(Experience.Chapters[i].Name);
                EditorGUILayout.EndHorizontal();
                if (!IsChapterNameUnique(i, Experience.Chapters[i]))
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(new GUIContent("Name must be unique !"), redLabelStyle);
                    GUILayout.Space(38);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }

                EditorGUILayout.BeginHorizontal();
                GUIContent helpIcon_5 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                helpIcon_5.tooltip = "This is the order of the chapter.";
                GUIStyle iconStyle_5 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                Rect rect_5 = GUILayoutUtility.GetRect(helpIcon_5, iconStyle_5);
                rect_5.width = EditorGUIUtility.labelWidth;
                GUI.Label(rect_5, helpIcon_5, iconStyle_5);
                EditorGUILayout.LabelField("Order:", GUILayout.Width(EditorGUIUtility.labelWidth));
                Experience.Chapters[i].Order = EditorGUILayout.IntField(Experience.Chapters[i].Order);
                EditorGUILayout.EndHorizontal();
                //DisplayLabeledIntField("Order:", "This is the order of the chapter.", Experience.ExperienceChapters[i].Order);
                GUILayout.Space(8);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(AddActionButton))
                {
                    AddAction(i);
                }
                if (GUILayout.Button(RemoveAllButton))
                {
                    var isOkRemoveAllActions = EditorUtility.DisplayDialog("Remove All Actions", "Do you really want to remove all actions ?", "Yes", "No");
                    if (isOkRemoveAllActions)
                    {
                        RemoveAllAction(i);
                    }
                }
                EditorGUILayout.EndHorizontal();
                DisplayActions(Experience.Chapters[i].Actions, i);
                InspectorCommon.DrawSeparationSpacing();
            }
            GUILayout.EndVertical();
            if (GUILayout.Button(new GUIContent(crossIcon), GUILayout.Width(20), GUILayout.Height(20))) // Delete button for action
            {
                RemoveChapterAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private void DisplayActions(List<ExperienceActionModel> actions, int chapterIndex)
    {
        EditorGUILayout.BeginVertical("box");
        for (int i = 0; i < actions.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            actions[i].isFoldout = InspectorCommon.DrawSectionHeader($"{actions[i].Name}", InspectorCommon.HeaderType.Level3, actions[i].isFoldout, true, $"");
            if (actions[i].isFoldout)
            {
                GUILayout.BeginVertical(EditorStyles.selectionRect, new GUILayoutOption[0]);
                GUILayout.Space(8);
                EditorGUILayout.LabelField($"Edit the details of the action", EditorStyles.boldLabel);
                GUILayout.Space(15);

                EditorGUILayout.BeginHorizontal();
                GUIContent helpIcon_3 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                helpIcon_3.tooltip = "This is the name of the action.";
                GUIStyle iconStyle_3 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                Rect rect_3 = GUILayoutUtility.GetRect(helpIcon_3, iconStyle_3);
                rect_3.width = EditorGUIUtility.labelWidth;
                GUI.Label(rect_3, helpIcon_3, iconStyle_3);
                EditorGUILayout.LabelField("Name:", GUILayout.Width(EditorGUIUtility.labelWidth));
                actions[i].Name = EditorGUILayout.TextField(actions[i].Name);
                EditorGUILayout.EndHorizontal();
                if (!IsActionNameUnique(chapterIndex, actions[i], i))
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(new GUIContent("Name must be unique !"), redLabelStyle);
                    GUILayout.Space(10);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }
                EditorGUILayout.BeginHorizontal();
                GUIContent helpIcon_0 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                helpIcon_0.tooltip = "The value sent when executing the action can be of type Boolean, String, Integer, or Float";
                GUIStyle iconStyle_0 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                Rect rect_0 = GUILayoutUtility.GetRect(helpIcon_0, iconStyle_0);
                rect_0.width = EditorGUIUtility.labelWidth;
                GUI.Label(rect_0, helpIcon_0, iconStyle_0);
                EditorGUILayout.LabelField("Action Value Type:", GUILayout.Width(EditorGUIUtility.labelWidth));
                EditorGUI.BeginChangeCheck();
                actions[i].ActionType = (ActionTypeEnum)EditorGUILayout.EnumPopup(actions[i].ActionType);
                if (EditorGUI.EndChangeCheck())
                    ClearActionDataOnChange(actions[i]);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUIContent helpIcon_1 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                helpIcon_1.tooltip = "The different response types available for your actions. They determine how users can answer";
                GUIStyle iconStyle_1 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                Rect rect_1 = GUILayoutUtility.GetRect(helpIcon_1, iconStyle_1);
                rect_1.width = EditorGUIUtility.labelWidth;
                GUI.Label(rect_1, helpIcon_1, iconStyle_1);
                EditorGUILayout.LabelField("Response Type:", GUILayout.Width(EditorGUIUtility.labelWidth));
                ActionTypeEnum selectedType = actions[i].ActionType;
                ActionResponseTypeEnum selectedResponseType = actions[i].ActionResponseTypeEnum;
                ActionResponseTypeEnum[] allowedResponseTypes;
                if (selectedType == ActionTypeEnum.String)
                {
                    allowedResponseTypes = new ActionResponseTypeEnum[] { ActionResponseTypeEnum.Unique, ActionResponseTypeEnum.MultipleValues };
                }
                else if (selectedType == ActionTypeEnum.Boolean)
                {
                    allowedResponseTypes = new ActionResponseTypeEnum[] { ActionResponseTypeEnum.Unique };
                }
                else
                {
                    allowedResponseTypes = new ActionResponseTypeEnum[] {
                        ActionResponseTypeEnum.Unique,
                        ActionResponseTypeEnum.MultipleValues,
                        ActionResponseTypeEnum.Interval,
                        ActionResponseTypeEnum.RatingLevel
                    };
                }
                int selectedResponseIndex = Array.IndexOf(allowedResponseTypes, selectedResponseType);
                EditorGUI.BeginChangeCheck();
                selectedResponseIndex = EditorGUILayout.Popup(selectedResponseIndex, allowedResponseTypes.Select(e => e.ToString()).ToArray());
                actions[i].ActionResponseTypeEnum = allowedResponseTypes[selectedResponseIndex];
                if (EditorGUI.EndChangeCheck())
                    ClearActionDataOnChange(actions[i], true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUIContent helpIcon_2 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                helpIcon_2.tooltip = "Check if this action is mandatory.\nA non-mandatory action can be passed by the user";
                GUIStyle iconStyle_2 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                Rect rect_2 = GUILayoutUtility.GetRect(helpIcon_2, iconStyle_2);
                rect_2.width = EditorGUIUtility.labelWidth;
                GUI.Label(rect_2, helpIcon_2, iconStyle_2);
                EditorGUILayout.Space(12);
                EditorGUILayout.LabelField("Is Mandatory:", GUILayout.Width(EditorGUIUtility.labelWidth));
                actions[i].IsMandatory = EditorGUILayout.Toggle(actions[i].IsMandatory);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUIContent helpIcon_5 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                helpIcon_5.tooltip = "Does this action depend on a previous action ?\nThe previous action must be completed before moving on to this one";
                GUIStyle iconStyle_6 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                Rect rect_6 = GUILayoutUtility.GetRect(helpIcon_5, iconStyle_6);
                rect_6.width = EditorGUIUtility.labelWidth;
                GUI.Label(rect_6, helpIcon_5, iconStyle_6);
                EditorGUILayout.Space(12);
                var actionModels = new[] { new ExperienceActionModel { Id = 1, Name = "No previous action" } }
                .Concat(Experience.Chapters[chapterIndex].Actions
                    .Where(x => actions.IndexOf(x) < actions.IndexOf(actions[i]))
                    .ToArray())
                .ToArray();
                EditorGUI.BeginChangeCheck();
                int selectedPreviousAction = actions[i].PreviousAction != null && actionModels.Any(x => x.Id == actions[i].PreviousAction.Id) ? Array.IndexOf(actionModels, actionModels.First(x => x.Id == actions[i].PreviousAction.Id)) : 0;
                selectedPreviousAction = EditorGUILayout.Popup("Previous action ?", selectedPreviousAction, actionModels.Select(x => x.Name).ToArray(), GUILayout.Width(300));
                if (EditorGUI.EndChangeCheck())
                {
                    if (selectedPreviousAction > 0)
                    {
                        actions[i].PreviousActionId = Experience.Chapters[chapterIndex].Actions.FirstOrDefault(x => x.Id == actionModels[selectedPreviousAction].Id).Id;
                        actions[i].PreviousAction = Experience.Chapters[chapterIndex].Actions.FirstOrDefault(x => x.Id == actions[i].PreviousActionId);
                        actions[i].HasPreviousAction = true;
                    }
                    else
                    {
                        actions[i].PreviousActionId = null;
                        actions[i].PreviousAction = null;
                        actions[i].HasPreviousAction = false;
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(8);
                InspectorCommon.DrawSeparationSpacingLine();
                EditorGUILayout.LabelField($"Response Definition", EditorStyles.boldLabel);
                EditorGUILayout.Space(8);
                switch (actions[i].ActionResponseTypeEnum)
                {
                    case ActionResponseTypeEnum.Unique:
                        EditorGUILayout.BeginHorizontal();
                        GUIContent helpIcon_Unique = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                        helpIcon_Unique.tooltip = UniqueExplanation;
                        GUIStyle iconStyle_5 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                        Rect rect_5 = GUILayoutUtility.GetRect(helpIcon_Unique, iconStyle_5);
                        rect_5.width = EditorGUIUtility.labelWidth;
                        GUI.Label(rect_5, helpIcon_Unique, iconStyle_5);
                        EditorGUILayout.LabelField("Unique", EditorStyles.boldLabel);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(8);
                        EditorGUILayout.BeginHorizontal();
                        if (actions[i].ActionType == ActionTypeEnum.Boolean)
                        {
                            string[] booleanOptions = new[] { "True", "False" }; // Predefined options
                            int currentIndex = Array.IndexOf(booleanOptions, !string.IsNullOrWhiteSpace(actions[i].UniqueGoodAnswer) ? (bool.Parse(actions[i].UniqueGoodAnswer) ? "True" : "False") : "True");
                            if (string.IsNullOrWhiteSpace(actions[i].UniqueGoodAnswer) && booleanOptions[currentIndex] == "True")
                                actions[i].UniqueGoodAnswer = "True";
                            EditorGUI.BeginChangeCheck();
                            int newIndex = EditorGUILayout.Popup("Right Answer:", currentIndex, booleanOptions);
                            if (EditorGUI.EndChangeCheck())
                                actions[i].UniqueGoodAnswer = booleanOptions[newIndex];
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Right Answer:", GUILayout.Width(EditorGUIUtility.labelWidth));
                            actions[i].UniqueGoodAnswer = EditorGUILayout.TextField(actions[i].UniqueGoodAnswer);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        GUIContent helpIcon_4 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                        helpIcon_4.tooltip = ScoreExplanation;
                        GUIStyle iconStyle_4 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                        Rect rect_4 = GUILayoutUtility.GetRect(helpIcon_4, iconStyle_4);
                        rect_4.width = EditorGUIUtility.labelWidth;
                        GUI.Label(rect_4, helpIcon_4, iconStyle_4);
                        EditorGUILayout.LabelField("Wrong Answer Score :", GUILayout.Width(EditorGUIUtility.labelWidth));
                        actions[i].WrongUniqueAnswerScore = Mathf.Clamp(actions[i].WrongUniqueAnswerScore, 0, 100);
                        actions[i].WrongUniqueAnswerScore = EditorGUILayout.IntField(actions[i].WrongUniqueAnswerScore);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ActionResponseTypeEnum.Interval:
                        EditorGUILayout.BeginHorizontal();
                        GUIContent helpIcon_Interval1 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                        helpIcon_Interval1.tooltip = IntervalExplanation;
                        GUIStyle iconStyle_Interval1 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                        Rect rect_Interval1 = GUILayoutUtility.GetRect(helpIcon_Interval1, iconStyle_Interval1);
                        rect_Interval1.width = EditorGUIUtility.labelWidth;
                        GUI.Label(rect_Interval1, helpIcon_Interval1, iconStyle_Interval1);
                        EditorGUILayout.LabelField("Interval", EditorStyles.boldLabel);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(8);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Expected value :", GUILayout.Width(EditorGUIUtility.labelWidth));
                        actions[i].MaxScoreResponse = EditorGUILayout.TextField(actions[i].MaxScoreResponse);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Minimum value :", GUILayout.Width(EditorGUIUtility.labelWidth));
                        actions[i].MinScoreNegativeResponse = EditorGUILayout.TextField(actions[i].MinScoreNegativeResponse);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Maximum value :", GUILayout.Width(EditorGUIUtility.labelWidth));
                        actions[i].MinScorePositiveResponse = EditorGUILayout.TextField(actions[i].MinScorePositiveResponse);
                        EditorGUILayout.EndHorizontal();
                        break;
                    case ActionResponseTypeEnum.RatingLevel:
                        EditorGUILayout.BeginHorizontal();
                        GUIContent helpIcon_RatingLevel1 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                        helpIcon_RatingLevel1.tooltip = RatingLevelExplanation;
                        GUIStyle iconStyle_RatingLevel1 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                        Rect rect_RatingLevel1 = GUILayoutUtility.GetRect(helpIcon_RatingLevel1, iconStyle_RatingLevel1);
                        rect_RatingLevel1.width = EditorGUIUtility.labelWidth;
                        GUI.Label(rect_RatingLevel1, helpIcon_RatingLevel1, iconStyle_RatingLevel1);
                        EditorGUILayout.LabelField("Rating Level", EditorStyles.boldLabel);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(8);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(AddRatingLevelButton))
                        {
                            AddRatingLevel(chapterIndex, i);
                        }
                        if (GUILayout.Button(RemoveAllButton))
                        {
                            var isOkRemoveAllRatingLevels = EditorUtility.DisplayDialog("Remove All Rating Levels", "Do you really want to remove all rating levels ?", "Yes", "No");
                            if (isOkRemoveAllRatingLevels)
                            {
                                RemoveAllRatingLevel(chapterIndex, i);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginVertical("box");
                        if (actions[i].ExperienceActionRatingLevels != null)
                        {
                            for (int j = 0; j < actions[i].ExperienceActionRatingLevels.Count; j++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                                actions[i].ExperienceActionRatingLevels[j].isFoldout = InspectorCommon.DrawSectionHeader($"Rating Level {j}", InspectorCommon.HeaderType.Level4, actions[i].ExperienceActionRatingLevels[j].isFoldout, true, $"");
                                if (actions[i].ExperienceActionRatingLevels[j].isFoldout)
                                {
                                    EditorGUILayout.Space(8);
                                    GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                                    EditorGUILayout.BeginHorizontal();
                                    GUIContent helpIcon_10 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                                    helpIcon_10.tooltip = ScoreExplanation;
                                    GUIStyle iconStyle_10 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                                    Rect rect_10 = GUILayoutUtility.GetRect(helpIcon_10, iconStyle_10);
                                    rect_10.width = EditorGUIUtility.labelWidth;
                                    GUI.Label(rect_10, helpIcon_10, iconStyle_10);
                                    EditorGUILayout.LabelField("Given Score :", GUILayout.Width(EditorGUIUtility.labelWidth));
                                    actions[i].ExperienceActionRatingLevels[j].GivenScore = Mathf.Clamp(actions[i].ExperienceActionRatingLevels[j].GivenScore, 0, 100);
                                    actions[i].ExperienceActionRatingLevels[j].GivenScore = EditorGUILayout.IntField(actions[i].ExperienceActionRatingLevels[j].GivenScore);
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Lower Limit :", GUILayout.Width(EditorGUIUtility.labelWidth));
                                    actions[i].ExperienceActionRatingLevels[j].LowerLimit = EditorGUILayout.TextField(actions[i].ExperienceActionRatingLevels[j].LowerLimit);
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Upper Limit :", GUILayout.Width(EditorGUIUtility.labelWidth));
                                    actions[i].ExperienceActionRatingLevels[j].UpperLimit = EditorGUILayout.TextField(actions[i].ExperienceActionRatingLevels[j].UpperLimit);
                                    EditorGUILayout.EndHorizontal();
                                    GUILayout.EndVertical();
                                    InspectorCommon.DrawSeparationSpacing();
                                }
                                GUILayout.EndVertical();
                                if (GUILayout.Button(new GUIContent(crossIcon), GUILayout.Width(20), GUILayout.Height(20))) // Delete button for action
                                {
                                    RemoveRatingLevelAtIndex(chapterIndex, i, j);
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                        }
                        EditorGUILayout.EndVertical();
                        break;
                    case ActionResponseTypeEnum.MultipleValues:
                        EditorGUILayout.BeginHorizontal();
                        GUIContent helpIcon_Values1 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                        helpIcon_Values1.tooltip = MultipleValuesExplanation;
                        GUIStyle iconStyle_Values1 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                        Rect rect_Values1 = GUILayoutUtility.GetRect(helpIcon_Values1, iconStyle_Values1);
                        rect_Values1.width = EditorGUIUtility.labelWidth;
                        GUI.Label(rect_Values1, helpIcon_Values1, iconStyle_Values1);
                        EditorGUILayout.LabelField("Multiple Values", EditorStyles.boldLabel);
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(8);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(AddValueButton))
                        {
                            AddValue(chapterIndex, i);
                        }
                        if (GUILayout.Button(RemoveAllButton))
                        {
                            var isOkRemoveAllValues = EditorUtility.DisplayDialog("Remove All Values", "Do you really want to remove all values ?", "Yes", "No");
                            if (isOkRemoveAllValues)
                            {
                                RemoveAllValues(chapterIndex, i);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginVertical("box");
                        if (actions[i].MultipleValues != null)
                        {
                            for (int j = 0; j < actions[i].MultipleValues.Count; j++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                                actions[i].MultipleValues[j].isFoldout = InspectorCommon.DrawSectionHeader($"Value {j}", InspectorCommon.HeaderType.Level4, actions[i].MultipleValues[j].isFoldout, true, $"");
                                if (actions[i].MultipleValues[j].isFoldout)
                                {
                                    EditorGUILayout.Space(8);
                                    GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
                                    EditorGUILayout.BeginHorizontal();
                                    if (actions[i].ActionType == ActionTypeEnum.Boolean)
                                    {
                                        string[] booleanOptions = new[] { "True", "False" }; // Predefined options
                                        int currentIndex = Array.IndexOf(booleanOptions, !string.IsNullOrWhiteSpace(actions[i].MultipleValues[j].GivenResponse) ? (bool.Parse(actions[i].MultipleValues[j].GivenResponse) ? "True" : "False") : "True");
                                        int newIndex = EditorGUILayout.Popup("Right Answer:", currentIndex, booleanOptions);
                                        actions[i].MultipleValues[j].GivenResponse = booleanOptions[newIndex];
                                    }
                                    else
                                    {
                                        EditorGUILayout.LabelField("Given Response :", GUILayout.Width(EditorGUIUtility.labelWidth));
                                        actions[i].MultipleValues[j].GivenResponse = EditorGUILayout.TextField(actions[i].MultipleValues[j].GivenResponse);
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.BeginHorizontal();
                                    GUIContent helpIcon_10 = new GUIContent(EditorGUIUtility.IconContent("_Help"));
                                    helpIcon_10.tooltip = ScoreExplanation;
                                    GUIStyle iconStyle_10 = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                                    Rect rect_10 = GUILayoutUtility.GetRect(helpIcon_10, iconStyle_10);
                                    rect_10.width = EditorGUIUtility.labelWidth;
                                    GUI.Label(rect_10, helpIcon_10, iconStyle_10);
                                    EditorGUILayout.LabelField("Score :", GUILayout.Width(EditorGUIUtility.labelWidth));
                                    actions[i].MultipleValues[j].Score = Mathf.Clamp(actions[i].MultipleValues[j].Score, 0, 100);
                                    actions[i].MultipleValues[j].Score = EditorGUILayout.IntField(actions[i].MultipleValues[j].Score);
                                    EditorGUILayout.EndHorizontal();
                                    GUILayout.EndVertical();
                                    InspectorCommon.DrawSeparationSpacing();
                                }
                                GUILayout.EndVertical();
                                if (GUILayout.Button(new GUIContent(crossIcon), GUILayout.Width(20), GUILayout.Height(20))) // Delete button for action
                                {
                                    RemoveValuesAtIndex(chapterIndex, i, j);
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                        }
                        EditorGUILayout.EndVertical();
                        break;
                }
                GUILayout.Space(4);
                GUILayout.EndVertical();
                InspectorCommon.DrawSeparationSpacing();
            }
            GUILayout.EndVertical();
            if (GUILayout.Button(new GUIContent(crossIcon), GUILayout.Width(20), GUILayout.Height(20))) // Delete button for action
            {
                RemoveActionAtIndex(chapterIndex, i);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private void LoadExperienceModel(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            ExperienceModel loadedModel = JsonConvert.DeserializeObject<ExperienceModel>(json);
            if (loadedModel != null)
            {
                foreach (var chapter in loadedModel.Chapters)
                {
                    chapter.Id = -(loadedModel.Chapters.Count(x => x.Id <= 0));
                    foreach (var action in chapter.Actions)
                    {
                        var actNewId = -chapter.Actions.Count(x => x.Id <= 0);
                        var previousActions = chapter.Actions.Where(x => x.PreviousActionId == action.Id).ToList();
                        if (previousActions.Any())
                            foreach (var act in previousActions)
                                act.PreviousActionId = actNewId;
                        action.Id = actNewId;
                        action.ExperienceChapterId = chapter.Id;
                        switch (action.ActionResponseTypeEnum)
                        {
                            case ActionResponseTypeEnum.MultipleValues:
                                if (action.MultipleValues.Any())
                                    foreach (var val in action.MultipleValues)
                                    {
                                        val.Id = action.MultipleValues.IndexOf(val);
                                        val.ExperienceAction = new ExperienceActionInfoModel
                                        {
                                            Id = action.Id,
                                            ActionResponseTypeEnum = action.ActionResponseTypeEnum,
                                            ActionType = action.ActionType,
                                            IsMandatory = action.IsMandatory,
                                            Name = action.Name,
                                            HasPreviousAction = action.HasPreviousAction
                                        };
                                    }
                                break;
                            case ActionResponseTypeEnum.RatingLevel:
                                if (action.ExperienceActionRatingLevels.Any())
                                    foreach (var val in action.ExperienceActionRatingLevels)
                                    {
                                        val.Id = action.ExperienceActionRatingLevels.IndexOf(val);
                                        val.ExperienceAction = new ExperienceActionInfoModel
                                        {
                                            Id = action.Id,
                                            ActionResponseTypeEnum = action.ActionResponseTypeEnum,
                                            ActionType = action.ActionType,
                                            IsMandatory = action.IsMandatory,
                                            Name = action.Name,
                                            HasPreviousAction = action.HasPreviousAction
                                        };
                                    }
                                break;
                        }
                    }
                }
                InversiveService.SetExperience(loadedModel);
                Experience = loadedModel;
            }
            else
            {
                Debug.LogError("Failed to load Experience Model from JSON.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading Experience Model: " + ex.Message);
        }
    }

    private void AddChapter()
    {
        ExperienceChapterModel newChapter = new ExperienceChapterModel
        {
            Id = -(Experience.Chapters.Count(x => x.Id <= 0)),
            Name = $"New Chapter {Experience.Chapters.Count}",
            Order = Experience.Chapters.Count
        };

        Experience.Chapters.Add(newChapter);
    }

    private void AddAction(int chapterIndex)
    {
        if (chapterIndex >= 0 && chapterIndex < Experience.Chapters.Count)
        {
            ExperienceActionModel newAction = new ExperienceActionModel
            {
                Id = -Experience.Chapters[chapterIndex].Actions.Count(x => x.Id <= 0),
                Name = $"New Action {Experience.Chapters[chapterIndex].Actions.Count}",
                ExperienceChapterId = Experience.Chapters[chapterIndex].Id,
                ActionType = ActionTypeEnum.Boolean,
                ActionResponseTypeEnum = ActionResponseTypeEnum.Unique
            };
            Experience.Chapters[chapterIndex].Actions.Add(newAction);
        }
    }

    private void AddRatingLevel(int chapterIndex, int actionIndex)
    {
        if (chapterIndex >= 0 && chapterIndex < Experience.Chapters.Count)
        {
            if (actionIndex >= 0 && actionIndex < Experience.Chapters[chapterIndex].Actions.Count)
            {
                ExperienceActionRatingLevelModel newRatingLevel = new ExperienceActionRatingLevelModel()
                {
                    ExperienceAction = new ExperienceActionInfoModel()
                    {
                        Id = Experience.Chapters[chapterIndex].Actions[actionIndex].Id,
                        ActionType = Experience.Chapters[chapterIndex].Actions[actionIndex].ActionType,
                        ActionResponseTypeEnum = Experience.Chapters[chapterIndex].Actions[actionIndex].ActionResponseTypeEnum
                    }
                };
                switch (Experience.Chapters[chapterIndex].Actions[actionIndex].ActionType)
                {
                    case ActionTypeEnum.Float:
                        newRatingLevel.UpperLimit = "0,0";
                        newRatingLevel.LowerLimit = "0,0";
                        break;
                    case ActionTypeEnum.Integer:
                        newRatingLevel.UpperLimit = "0";
                        newRatingLevel.LowerLimit = "0";
                        break;
                }
                Experience.Chapters[chapterIndex].Actions[actionIndex].ExperienceActionRatingLevels.Add(newRatingLevel);
            }
        }
    }

    private void RemoveAllRatingLevel(int chapterIndex, int actionIndex)
    {
        Experience.Chapters[chapterIndex].Actions[actionIndex].ExperienceActionRatingLevels.Clear();
    }

    private void RemoveRatingLevelAtIndex(int chapterIndex, int actionIndex, int ratingLevelIndex)
    {

        List<ExperienceActionRatingLevelModel> ratingLevels = Experience.Chapters[chapterIndex].Actions[actionIndex].ExperienceActionRatingLevels;
        if (ratingLevelIndex >= 0 && ratingLevelIndex < ratingLevels.Count)
        {
            ratingLevels.RemoveAt(ratingLevelIndex);
        }
    }

    private void AddValue(int chapterIndex, int actionIndex)
    {
        if (chapterIndex >= 0 && chapterIndex < Experience.Chapters.Count)
        {
            if (actionIndex >= 0 && actionIndex < Experience.Chapters[chapterIndex].Actions.Count)
            {
                ExperienceActionValueModel newValue = new ExperienceActionValueModel()
                {
                    ExperienceAction = new ExperienceActionInfoModel()
                    {
                        Id = Experience.Chapters[chapterIndex].Actions[actionIndex].Id,
                        ActionType = Experience.Chapters[chapterIndex].Actions[actionIndex].ActionType,
                        ActionResponseTypeEnum = Experience.Chapters[chapterIndex].Actions[actionIndex].ActionResponseTypeEnum
                    }
                };
                switch (Experience.Chapters[chapterIndex].Actions[actionIndex].ActionType)
                {
                    case ActionTypeEnum.Boolean:
                        newValue.GivenResponse = "True";
                        break;
                    case ActionTypeEnum.String:
                        newValue.GivenResponse = "Veuillez saisir une r�ponse";
                        break;
                    case ActionTypeEnum.Float:
                        newValue.GivenResponse = "0,0";
                        break;
                    case ActionTypeEnum.Integer:
                        newValue.GivenResponse = "0";
                        break;
                }
                Experience.Chapters[chapterIndex].Actions[actionIndex].MultipleValues.Add(newValue);
            }
        }
    }

    private void RemoveAllValues(int chapterIndex, int actionIndex)
    {
        Experience.Chapters[chapterIndex].Actions[actionIndex].MultipleValues.Clear();
    }

    private void RemoveValuesAtIndex(int chapterIndex, int actionIndex, int valueIndex)
    {
        List<ExperienceActionValueModel> mutlipleValues = Experience.Chapters[chapterIndex].Actions[actionIndex].MultipleValues;
        if (valueIndex >= 0 && valueIndex < mutlipleValues.Count)
        {
            mutlipleValues.RemoveAt(valueIndex);
        }
    }

    private void RemoveChapterAtIndex(int index)
    {
        if (index >= 0 && index < Experience.Chapters.Count)
        {
            Experience.Chapters.RemoveAt(index);
        }
    }

    private void ExportModel()
    {
        try
        {
            var json = JsonConvert.SerializeObject(Experience);
            string assetsFolderPath = Application.dataPath + "/Inversive SDK/";
            if (!Directory.Exists(assetsFolderPath))
                Directory.CreateDirectory(assetsFolderPath);
            string filePath = Path.Combine(assetsFolderPath, "Experience.json");
            File.WriteAllText(filePath, json);
            if (EditorUtility.DisplayDialog("Model Exported", $"Your model has been exported in your Assets folder : Assets/Inversive SDK/Experience.json", "Ok"))
            {
                Debug.Log("Model exported to JSON in Assets folder: " + "Experience.json");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error exporting model: " + ex.Message);
        }
    }

    public void UpdateModel()
    {
        EditorCoroutineUtility.StartCoroutine(GetExperienceHead(), this);
    }

    private IEnumerator GetExperienceHead()
    {
        using (var req = UnityWebRequest.Get(GetUri($"Account/get-experience")))
        {
            req.method = "Get";
            req.SetRequestHeader("AccessToken", InversiveService.GetAccessToken());
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(InversiveUtilities.Message("Successfully updated model !"));
                ExperienceModel loadedModel = JsonConvert.DeserializeObject<ExperienceModel>(req.downloadHandler.text);
                if (loadedModel != null)
                {
                    InversiveService.SetExperience(loadedModel);
                    InversiveService.SetExperienceHead(loadedModel);
                    Experience = loadedModel;
                    foreach (var chapter in Experience.Chapters)
                        chapter.Actions = chapter.Actions.OrderBy(x => x.PreviousActionId).ToList();
                }
                else
                {
                    Debug.LogError(InversiveUtilities.Message("Failed to update Experience Model."));
                }
            }
            else
            {
                Debug.LogError(InversiveUtilities.Message("Failed to update Experience Model."));
            }
        }
        yield return null;
    }

    private void CancelChanges()
    {
        if (InversiveService.GetExperience() != null)
        {
            Experience = InversiveService.GetExperienceHead();
            var json = JsonConvert.SerializeObject(Experience);
            PlayerPrefs.SetString("ExperienceLocal", json);
        }
    }

    private void ResetModel()
    {
        Experience = new();
    }

    private void SaveDataToApi()
    {
        EditorCoroutineUtility.StartCoroutine(SaveExperienceModel(Experience, (x) =>
        {
            if (x.result != UnityWebRequest.Result.Success)
                Debug.LogError(InversiveUtilities.Message($"Save failed ! : \n\n {x.error}"));
            else
            {
                Debug.Log(InversiveUtilities.Message("Save succeed !"));
                EditorCoroutineUtility.StartCoroutine(GetExperienceHead(), this);
                Close();
                EditorUtility.DisplayDialog("Saved successfully !", "This will be the model of reference on the remote experience model and so for the other developers", "Ok");
            }
        }), this);
    }

    private static IEnumerator SaveExperienceModel(ExperienceModel model, Action<UnityWebRequest> request = null)
    {
        if (model != null)
        {
            model = ReworkModel(model);
            if (!string.IsNullOrEmpty(InversiveService.GetAccessToken()))
            {
                var json = JsonConvert.SerializeObject(model);
                using (var req = UnityWebRequest.Put(GetUri($"Account/save-experience"), json))
                {
                    req.method = "POST";
                    req.SetRequestHeader("Content-Type", "application/json");
                    req.SetRequestHeader("AccessToken", InversiveService.GetAccessToken());
                    yield return req.SendWebRequest();
                    request(req);
                }
            }
        }
        yield return null;
    }

    private static ExperienceModel ReworkModel(ExperienceModel model)
    {
        if (model.Chapters.Any())
        {
            foreach (var chapter in model.Chapters)
            {
                foreach (var action in chapter.Actions)
                {
                    if (action.ActionResponseTypeEnum != ActionResponseTypeEnum.Unique)
                        action.UniqueGoodAnswer = null;
                }
            }
        }
        return model;
    }

    private static Uri GetUri(string relativePath)
    {
        var baseUri = new Uri(InversiveUtilities.GetApiUrl());
        return new Uri(baseUri, relativePath);
    }

    private void RemoveAllChapter()
    {
        Experience.Chapters.Clear();
    }

    private void RemoveActionAtIndex(int chapterIndex, int actionIndex)
    {
        if (chapterIndex >= 0 && chapterIndex < Experience.Chapters.Count)
        {
            List<ExperienceActionModel> actions = Experience.Chapters[chapterIndex].Actions;
            if (actionIndex >= 0 && actionIndex < actions.Count)
            {
                actions.RemoveAt(actionIndex);
            }
        }
    }
    private void RemoveAllAction(int chapterIndex)
    {
        Experience.Chapters[chapterIndex].Actions.Clear();
    }

    private void LoadData(bool afterLogin = false)
    {
        // Load existing data from your SDK or storage and populate 'chapters' list
        if (afterLogin)
            UpdateModel();
        else
            Experience = InversiveService.GetExperience();
    }

    private bool IsActionNameUnique(int chapterIndex, ExperienceActionModel action, int actionIndex)
    {
        if (Experience.Chapters[chapterIndex].Actions.Where((action, index) => index != actionIndex).Any(x => x.Name == action.Name))
        {
            return false;
        }

        return true;
    }

    private static string[] ScoringEnumDescriptions = new string[]
    {
        "Hundred : 0 to 100",
        "Ten : 0 to 10",
        "Twenty : 0 to 20",
        "Letter : A to F"
    };

    private bool IsChapterNameUnique(int chapterIndex, ExperienceChapterModel chapter)
    {
        if (Experience.Chapters.Where((chapter, index) => index != chapterIndex).Any(x => x.Name == chapter.Name))
        {
            return false;
        }

        return true;
    }

    private void ClearActionDataOnChange(ExperienceActionModel action, bool IsResponseTypeChanged = false)
    {
        if (action.ActionResponseTypeEnum == ActionResponseTypeEnum.Unique)
        {
            switch (action.ActionType)
            {
                case ActionTypeEnum.Boolean:
                    action.UniqueGoodAnswer = "True";
                    break;
                case ActionTypeEnum.String:
                    action.UniqueGoodAnswer = "Veuillez saisir une r�ponse";
                    break;
                case ActionTypeEnum.Float:
                    action.UniqueGoodAnswer = "0,0";
                    break;
                case ActionTypeEnum.Integer:
                    action.UniqueGoodAnswer = "0";
                    break;
            }
        }
        else if (action.ActionResponseTypeEnum == ActionResponseTypeEnum.Interval)
        {
            switch (action.ActionType)
            {
                case ActionTypeEnum.Float:
                    action.MaxScoreResponse = "0,0";
                    action.MinScorePositiveResponse = "0,0";
                    action.MinScoreNegativeResponse = "0,0";
                    break;
                case ActionTypeEnum.Integer:
                    action.MaxScoreResponse = "0";
                    action.MinScorePositiveResponse = "0";
                    action.MinScoreNegativeResponse = "0";
                    break;
            }
        }
        if (!IsResponseTypeChanged)
        {
            if (action.ActionResponseTypeEnum == ActionResponseTypeEnum.MultipleValues)
            {
                foreach (var value in action.MultipleValues)
                {
                    switch (action.ActionType)
                    {
                        case ActionTypeEnum.Boolean:
                            value.GivenResponse = "True";
                            break;
                        case ActionTypeEnum.String:
                            value.GivenResponse = "Veuillez saisir une r�ponse";
                            break;
                        case ActionTypeEnum.Float:
                            value.GivenResponse = "0,0";
                            break;
                        case ActionTypeEnum.Integer:
                            value.GivenResponse = "0";
                            break;
                    }
                }

            }
            else if (action.ActionResponseTypeEnum == ActionResponseTypeEnum.RatingLevel)
            {
                foreach (var value in action.ExperienceActionRatingLevels)
                {
                    switch (action.ActionType)
                    {
                        case ActionTypeEnum.Float:
                            value.UpperLimit = "0,0";
                            value.LowerLimit = "0,0";
                            break;
                        case ActionTypeEnum.Integer:
                            value.UpperLimit = "0";
                            value.LowerLimit = "0";
                            break;
                    }
                }
            }
        }
        else
        {
            if (action.ActionResponseTypeEnum != ActionResponseTypeEnum.Interval)
            {
                action.MaxScoreResponse = null;
                action.MinScoreNegativeResponse = null;
                action.MinScorePositiveResponse = null;
            }
            if (action.ActionResponseTypeEnum != ActionResponseTypeEnum.MultipleValues)
                action.MultipleValues.Clear();
            if (action.ActionResponseTypeEnum != ActionResponseTypeEnum.RatingLevel)
                action.ExperienceActionRatingLevels.Clear();
        }
    }
}
