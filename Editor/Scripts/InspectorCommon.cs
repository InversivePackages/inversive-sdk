using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class InspectorCommon
{
    public const float INDENT = 10f;
    public const byte FLAG_OVERRIDE_OFF = 0;
    public const byte FLAG_OVERRIDE_TIMINGS = 1;
    public const byte FLAG_OVERRIDE_EASINGS = 2;
    public const string StatusMsg6 = "NOT READY: Requires a reference to a QueueProcessor.";
    public const string StatusMsg7 = "CAREFUL: QueueProcessor is available on this GameObject but is not linked.";
    public const string StatusMsg8 = "CAREFUL: QueueProcessor is available and linked, but it is disabled.";
    public const string StatusTip5 = "Requires an enabled QueueProcessor to function. The required QueueProcessor for this instance can exist on any GameObject but must be available immediately at runtime.\n\nWithout a QueueProcessor, SALSA will throw null reference errors.\n\nIf a linked-QueueProcessor is not enabled, animations will not function.";
    private static GUIStyle styleHasBackground = new GUIStyle();
    private static GUIStyle styleLeftAlign = new GUIStyle();
    private static GUIStyle styleRightAlign = new GUIStyle();
    private static GUIStyle styleBoldCentered = new GUIStyle();
    private static GUIStyle styleBoldRightLabel = new GUIStyle();
    private static GUIStyle styleBoxZeroPadLeft = new GUIStyle();
    private static GUIStyle styleBoxZeroPadRight = new GUIStyle();
    private static Color inspectorTextColor = new GUIStyle().normal.textColor;
    public static bool isInspectorLocked = false;
    private const string INDICATOR = "?";

    public static Color ClrHeaderWarn => Color.red;

    public static Color ClrBgError => Color.red;

    public static Color ClrBgWarn => Color.red;

    public static Color ClrBgNormal => Color.white;

    public static Color ClrBgGood => Color.green;

    public static Color ClrBgButton => Color.white;

    public static Color ClrHeaderLvl1 =>Color.clear;

    public static Color ClrHeaderLvl2 => new Color(60f / 255f, 131f / 255f, 246f / 255f);
    public static Color ClrHeaderLvl3 => new Color(249f / 255f, 144f / 255f, 57f / 255f);
    public static Color ClrHeaderLvl4 => new Color(204f / 255f, 122f / 255f, 239f / 255f);
    public static Color ClrHeaderLvl3Disabled => Color.white;

    public static GUIStyle StyleRightAlign
    {
        get
        {
            InspectorCommon.styleRightAlign.alignment = (TextAnchor)8;
            return InspectorCommon.styleRightAlign;
        }
    }

    public static GUIStyle StyleHasBackground
    {
        get
        {
            InspectorCommon.styleHasBackground.normal.background = new Texture2D(1, 1);
            if (InspectorCommon.styleHasBackground.padding.left != 5)
                InspectorCommon.styleHasBackground.padding = new RectOffset(5, 5, 3, 3);
            return InspectorCommon.styleHasBackground;
        }
    }

    public static GUIStyle StyleLeftAlign
    {
        get
        {
            InspectorCommon.styleLeftAlign.alignment = (TextAnchor)3;
            return InspectorCommon.styleLeftAlign;
        }
    }

    public static GUIStyle StyleBoldCentered
    {
        get
        {
            InspectorCommon.styleBoldCentered.normal.background = new Texture2D(1, 1);
            InspectorCommon.styleBoldCentered.alignment = (TextAnchor)4;
            InspectorCommon.styleBoldCentered.fontStyle = (FontStyle)1;
            InspectorCommon.styleBoldCentered.fontSize = 10;
            return InspectorCommon.styleBoldCentered;
        }
    }

    public static GUIStyle StyleBoldRightLabel
    {
        get
        {
            InspectorCommon.styleBoldRightLabel.alignment = (TextAnchor)8;
            InspectorCommon.styleBoldRightLabel.fontStyle = (FontStyle)1;
            InspectorCommon.styleBoldRightLabel.padding = new RectOffset(0, 2, 7, 2);
            InspectorCommon.styleBoldRightLabel.fontSize = 11;
            return InspectorCommon.styleBoldRightLabel;
        }
    }

    public static GUIStyle StyleBoxZeroPadLeft
    {
        get
        {
            InspectorCommon.styleBoxZeroPadLeft.margin = new RectOffset(0, 4, 4, 4);
            InspectorCommon.styleBoxZeroPadLeft.padding = new RectOffset(0, 3, 3, 3);
            return InspectorCommon.styleBoxZeroPadLeft;
        }
    }

    public static GUIStyle StyleBoxZeroPadRight
    {
        get
        {
            InspectorCommon.styleBoxZeroPadRight.margin = new RectOffset(4, 0, 4, 4);
            InspectorCommon.styleBoxZeroPadRight.padding = new RectOffset(3, 0, 3, 3);
            return InspectorCommon.styleBoxZeroPadRight;
        }
    }

    public static Color InspectorTextColor => InspectorCommon.inspectorTextColor;

    public static bool DrawSectionHeader(
      string title,
      InspectorCommon.HeaderType type,
      bool state,
      bool configGood,
      string extraMsg = "",
      bool enabled = true)
    {
        Color backgroundColor = GUI.backgroundColor;
        title = (state ? "▼ " : "► ") + title;
        extraMsg = "<color=#ffffffff>" + extraMsg + "</color>";
        string str;
        Color color;
        switch (type)
        {
            case InspectorCommon.HeaderType.Level1:
                str = "12";
                color = InspectorCommon.ClrHeaderLvl1;
                title = (configGood ? "<color=#ddddddff>" : "<color=#000000ff>") + title + "</color>";
                break;
            case InspectorCommon.HeaderType.Level2:
                str = "11";
                color = InspectorCommon.ClrHeaderLvl2;
                title = (configGood ? "<color=#ddddddff>" : "<color=#000000ff>") + title + "</color>";
                break;
            case InspectorCommon.HeaderType.Level3:
                str = "11";
                color = InspectorCommon.ClrHeaderLvl3;
                title = (configGood ? "<color=#ddddddff>" : "<color=#000000ff>") + title + "</color>";
                break;
            case InspectorCommon.HeaderType.Level4:
                str = "11";
                color = InspectorCommon.ClrHeaderLvl4;
                title = (configGood ? "<color=#ddddddff>" : "<color=#000000ff>") + title + "</color>";
                break;
            default:
                str = "10";
                color = enabled ? InspectorCommon.ClrHeaderLvl4 : InspectorCommon.ClrHeaderLvl3Disabled;
                title = (configGood ? "<color=#ddddddff>" : "<color=#000000ff>") + title + "</color>";
                break;
        }
        extraMsg = "<size=" + str + ">" + extraMsg + "</size>";
        title = "<size=" + str + ">" + title + "</size>";
        GUI.backgroundColor = color;
        GUILayout.BeginHorizontal(InspectorCommon.StyleHasBackground, new GUILayoutOption[0]);
        if (!GUILayout.Toggle(true, title, InspectorCommon.StyleLeftAlign, new GUILayoutOption[2]
        {
        GUILayout.MinWidth(60f),
        GUILayout.ExpandWidth(true)
        }))
            state = !state;
        if (configGood && !GUILayout.Toggle(true, extraMsg, InspectorCommon.StyleRightAlign, new GUILayoutOption[0]))
            state = !state;
        GUILayout.EndHorizontal();
        GUI.backgroundColor = backgroundColor;
        return state;
    }

    public static bool DrawLabelToggle(string label, bool state, GUIStyle option)
    {
        if (!GUILayout.Toggle(true, new GUIContent(label, "Click to toggle feature."), option, new GUILayoutOption[0]))
            state = !state;
        return state;
    }

    public static bool DrawItemButton(string text, string tip, float width = 25f, float height = 18f) => GUILayout.Button(new GUIContent(text, tip), EditorStyles.toolbarButton, new GUILayoutOption[2]
    {
      GUILayout.Width(width),
      GUILayout.Height(height)
    });

    public static void DrawItemNonButton(string text, string tip, float width = 25f) => GUILayout.Label(new GUIContent(text, tip), EditorStyles.toolbarButton, new GUILayoutOption[1]
    {
      GUILayout.Width(width)
    });

    public static bool DrawFunctionButton(GUIContent content, int size, Color bg, float width)
    {
        GUI.backgroundColor = bg;
        return GUILayout.Button(content, new GUIStyle(GUI.skin.button)
        {
            fontSize = size,
            normal = {
          textColor = Color.white
        }
        }, new GUILayoutOption[1] { GUILayout.MaxWidth(width) });
    }

    public static void DrawStatusMessageBlock(string msg, string tip, Color clr)
    {
        GUILayout.Space(5f);
        GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
        GUIStyle guiStyle = new GUIStyle();
        Color textColor = guiStyle.normal.textColor;
        guiStyle.wordWrap = true;
        guiStyle.normal.textColor = clr;
        GUILayout.Label(new GUIContent(msg, tip), guiStyle, new GUILayoutOption[0]);
        guiStyle.normal.textColor = textColor;
        GUILayout.EndVertical();
        GUILayout.Space(5f);
    }

    public static void DrawBackgroundCondition(bool isGood)
    {
        if (isGood)
            GUI.backgroundColor = InspectorCommon.ClrBgGood;
        else
            GUI.backgroundColor = InspectorCommon.ClrBgError;
    }

    public static void DrawBackgroundCondition(InspectorCommon.AlertType alertType)
    {
        switch (alertType)
        {
            case InspectorCommon.AlertType.Good:
                GUI.backgroundColor = InspectorCommon.ClrBgGood;
                break;
            case InspectorCommon.AlertType.Warning:
                GUI.backgroundColor = InspectorCommon.ClrBgWarn;
                break;
            case InspectorCommon.AlertType.Error:
                GUI.backgroundColor = InspectorCommon.ClrBgError;
                break;
            default:
                GUI.backgroundColor = InspectorCommon.ClrBgNormal;
                break;
        }
    }

    public static void DrawResetBg() => GUI.backgroundColor = Color.white;

    public static void DrawSeparationSpacingLine(float spacing = 3f)
    {
        GUILayout.Space(spacing);
        GUILayout.Box("", new GUILayoutOption[2]
        {
        GUILayout.ExpandWidth(true),
        GUILayout.Height(3f)
        });
        GUILayout.Space(spacing);
    }

    private static void DrawHorizontalLine(Color color, int thickness = 2, int padding = 20)
    {
        Rect line = EditorGUILayout.GetControlRect(false, thickness, GUILayout.Height(thickness));
        EditorGUI.DrawRect(new Rect(line.x - padding, line.y, line.width + padding * 2, thickness), color);
    }

    public static void DrawSeparationSpacing(float spacing = 2f) => GUILayout.Space(spacing);

    //public static void DrawExpressionComponent(DrawExpressionData drawExpressionData)
    //{
    //    List<ExpressionComponent> components = drawExpressionData.ExpData.components;
    //    float currentViewWidth = EditorGUIUtility.currentViewWidth;
    //    InspectorCommon.DrawSeparationSpacing(5f);
    //    if (components.Count == 0)
    //    {
    //        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //        InspectorCommon.DrawStatusMessageBlock("NOT READY: No components found.", "Add one or more expression components as part of this expression.", Color.red);
    //    }
    //    else
    //    {
    //        for (int index = 0; index < components.Count; ++index)
    //        {
    //            InspectorControllerHelperData controllerVar = drawExpressionData.ExpData.controllerVars[index];
    //            if (!components[index].enabled)
    //                GUI.backgroundColor = SalsaUtil.ConvertColor((int)byte.MaxValue, 211, 211, (int)byte.MaxValue);
    //            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
    //            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //            ExpressionConfigurationState configState = InspectorCommon.ConfigurationCheckExpressionComponent(drawExpressionData.ExpData, index, drawExpressionData.InspDrawBoneCaptureControls);
    //            EditorGUI.BeginChangeCheck();
    //            components[index].enabled = (GUILayout.Toggle((components[index].enabled ? 1 : 0) != 0, new GUIContent("", "Uncheck to disable this component for all activating calls. Deactivating calls continue to operate."), new GUILayoutOption[1]
    //            {
    //        GUILayout.Width(12f)
    //            }) ? 1 : 0) != 0;
    //            if (EditorGUI.EndChangeCheck() && drawExpressionData.ExpData.previewDisplayMode)
    //                InspectorCommon.PreviewExpression(drawExpressionData.ExpData);
    //            string name = components[index].enabled ? components[index].name : "(disabled) " + components[index].name;
    //            components[index].inspFoldout = InspectorCommon.DrawSectionHeader(InspectorCommon.TruncateHeaderName(name, currentViewWidth, 380f), InspectorCommon.HeaderType.Level3, components[index].inspFoldout, configState.state, components[index].controlType.ToString(), components[index].enabled);
    //            if (!Application.isPlaying)
    //            {
    //                if (drawExpressionData.IsPasteMode)
    //                {
    //                    if (drawExpressionData.CopiedFrom.expression == drawExpressionData.Exp && drawExpressionData.CopiedFrom.component == index)
    //                    {
    //                        if (InspectorCommon.DrawItemButton("cancel", "Paste the copied expression component.", 50f))
    //                            drawExpressionData.CopyModeCallback(false, drawExpressionData.CopiedFrom);
    //                    }
    //                    else if (InspectorCommon.DrawItemButton("paste", "Paste the copied expression component.", 50f))
    //                    {
    //                        ExpressionDataIndicies pastedTo;
    //                        // ISSUE: explicit constructor call
    //                        ((ExpressionDataIndicies)ref pastedTo).\u002Ector(drawExpressionData.Exp, index);
    //                        drawExpressionData.PasteModeCallback(drawExpressionData.CopiedFrom, pastedTo);
    //                        drawExpressionData.CopyModeCallback(false, drawExpressionData.CopiedFrom);
    //                    }
    //                }
    //                else if (!drawExpressionData.ExpData.previewDisplayMode && InspectorCommon.DrawItemButton("copy", "Copy this expression component.", 50f))
    //                    drawExpressionData.CopyModeCallback(true, new ExpressionDataIndicies(drawExpressionData.Exp, index));
    //                if (!drawExpressionData.ExpData.previewDisplayMode && InspectorCommon.DrawItemButton("x", "Delete Item", 20f) && InspectorCommon.RemoveExpressionComponent(drawExpressionData.ExpData, index))
    //                    break;
    //            }
    //            GUILayout.EndHorizontal();
    //            if (components[index].inspFoldout)
    //            {
    //                InspectorCommon.DrawSeparationSpacing(5f);
    //                components[index].name = EditorGUILayout.TextField(new GUIContent("Name:", "Enter a friendly name for this expression component."), components[index].name, new GUILayoutOption[0]);
    //                if (drawExpressionData.InspDrawEyesDirectionType)
    //                    components[index].directionType = (ExpressionComponent.DirectionType)EditorGUILayout.EnumPopup(new GUIContent("Direction Type", "For components that have a directional element."), (Enum)(object)components[index].directionType, new GUILayoutOption[0]);
    //                InspectorCommon.DrawAnimationTimings(drawExpressionData.InspDrawTimingHold, drawExpressionData.InspDrawTimingOff, drawExpressionData.InspDrawTimingDelay, components[index].expressionType, ref components[index].durationDelay, ref components[index].durationOn, ref components[index].durationHold, ref components[index].durationOff, drawExpressionData.OverrideFlags);
    //                GUILayout.Space(3f);
    //                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                GUILayout.Space(15f);
    //                components[index].isSmoothDisable = GUILayout.Toggle(components[index].isSmoothDisable, new GUIContent("Disable Smoothly?", "When an expression component is disabled, should the animation be reset smoothly or instantly."), new GUILayoutOption[0]);
    //                GUILayout.EndHorizontal();
    //                if (components[index].controlType == 1 || components[index].controlType == null || components[index].controlType == 6)
    //                    InspectorCommon.DrawAnimatorControlledOption(components, index, drawExpressionData.InspDrawBoneOffsetOptions);
    //                InspectorCommon.DrawSeparationSpacing(5f);
    //                if (((int)drawExpressionData.OverrideFlags & 2) == 0)
    //                {
    //                    ++EditorGUI.indentLevel;
    //                    components[index].easing = (LerpEasings.EasingType)EditorGUILayout.EnumPopup(new GUIContent("Easing", "Select the animation acceleration mode for this expression component."), (Enum)(object)components[index].easing, new GUILayoutOption[0]);
    //                    if (components[index].expressionType == 3 || components[index].expressionType == 4)
    //                        components[index].frac = EditorGUILayout.Slider(new GUIContent((double)currentViewWidth < 430.0 ? "Dynamics" : "Component Dynamics", "Adjusts the range of overall expression component movement."), components[index].frac, 0.0f, 1f, new GUILayoutOption[0]);
    //                }
    //                else
    //                    InspectorCommon.DrawSettingsOverridden("Easings");
    //                if (components[index].expressionType != 2)
    //                {
    //                    InspectorCommon.DrawSeparationSpacing(10f);
    //                    EditorGUI.BeginChangeCheck();
    //                    ExpressionComponent.ControlType controlType1 = components[index].controlType;
    //                    string str;
    //                    if (components[index].expressionType == null)
    //                    {
    //                        components[index].lipsyncControlType = Expression.ConvertToLipsyncControlType(components[index].controlType.ToString());
    //                        components[index].lipsyncControlType = (ExpressionComponent.LipsyncControlType)EditorGUILayout.EnumPopup(new GUIContent((double)currentViewWidth < 380.0 ? "Controller" : "Controller Type", "Select the appropriate animation controller for this Lipsync expression component."), (Enum)(object)components[index].lipsyncControlType, new GUILayoutOption[0]);
    //                        str = components[index].lipsyncControlType.ToString();
    //                    }
    //                    else if (components[index].expressionType == 1)
    //                    {
    //                        components[index].emoteControlType = Expression.ConvertToEmoteControlType(components[index].controlType.ToString());
    //                        components[index].emoteControlType = (ExpressionComponent.EmoteControlType)EditorGUILayout.EnumPopup(new GUIContent((double)currentViewWidth < 380.0 ? "Controller" : "Controller Type", "Select the appropriate animation controller for this Lipsync expression component."), (Enum)(object)components[index].emoteControlType, new GUILayoutOption[0]);
    //                        str = components[index].emoteControlType.ToString();
    //                    }
    //                    else
    //                    {
    //                        components[index].eyesControlType = Expression.ConvertToEyesControlType(components[index].controlType.ToString());
    //                        components[index].eyesControlType = (ExpressionComponent.EyesControlType)EditorGUILayout.EnumPopup(new GUIContent((double)currentViewWidth < 380.0 ? "Controller" : "Controller Type", "Select the appropriate animation controller for this expression component."), (Enum)(object)components[index].eyesControlType, new GUILayoutOption[0]);
    //                        str = components[index].eyesControlType.ToString();
    //                    }
    //                    components[index].controlType = Expression.ConvertToGlobalControlType(str);
    //                    if (EditorGUI.EndChangeCheck())
    //                    {
    //                        if (components[index].controlType == 6 && Object.op_Equality((Object)drawExpressionData.Go.GetComponent<UmaUepProxy>(), (Object)null))
    //                            drawExpressionData.Go.AddComponent<UmaUepProxy>();
    //                        if (drawExpressionData.ExpData.previewDisplayMode)
    //                        {
    //                            ExpressionComponent.ControlType controlType2 = components[index].controlType;
    //                            components[index].controlType = controlType1;
    //                            drawExpressionData.ExpData.previewDisplayMode = false;
    //                            InspectorCommon.PreviewExpression(drawExpressionData.ExpData);
    //                            components[index].controlType = controlType2;
    //                        }
    //                        InspectorCommon.AttachToLocalControllerComponent(components[index].controlType, controllerVar, drawExpressionData.Go);
    //                    }
    //                }
    //                switch ((int)components[index].controlType)
    //                {
    //                    case 0:
    //                        if (configState.state)
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Normal);
    //                        else
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Error);
    //                        SkinnedMeshRenderer smr = controllerVar.smr;
    //                        int blendIndex1 = controllerVar.blendIndex;
    //                        EditorGUI.BeginChangeCheck();
    //                        controllerVar.smr = (SkinnedMeshRenderer)EditorGUILayout.ObjectField((double)currentViewWidth < 460.0 ? "SkinnedMesh" : "SkinnedMeshRenderer", (Object)controllerVar.smr, typeof(SkinnedMeshRenderer), true, new GUILayoutOption[0]);
    //                        if (EditorGUI.EndChangeCheck() && drawExpressionData.ExpData.previewDisplayMode)
    //                        {
    //                            smr.SetBlendShapeWeight(blendIndex1, 0.0f);
    //                            drawExpressionData.ExpData.previewDisplayMode = false;
    //                            InspectorCommon.PreviewExpression(drawExpressionData.ExpData);
    //                            drawExpressionData.ExpData.previewDisplayMode = true;
    //                        }
    //                        InspectorCommon.DrawResetBg();
    //                        if (configState.state)
    //                        {
    //                            EditorGUI.BeginChangeCheck();
    //                            controllerVar.blendIndex = EditorGUILayout.Popup((double)currentViewWidth < 410.0 ? "Blendshape" : "Blendshape Index", controllerVar.blendIndex, SalsaUtil.GetBlendShapes(controllerVar.smr), new GUILayoutOption[0]);
    //                            if (EditorGUI.EndChangeCheck() && drawExpressionData.ExpData.previewDisplayMode)
    //                            {
    //                                controllerVar.smr.SetBlendShapeWeight(blendIndex1, 0.0f);
    //                                InspectorCommon.PreviewExpression(drawExpressionData.ExpData);
    //                            }
    //                            if (((!drawExpressionData.ExpData.previewDisplayMode ? 0 : (Mathf.Approximately(drawExpressionData.Dynamics, 1f) ? 1 : 0)) | (!drawExpressionData.ExpData.previewDisplayMode ? 1 : 0)) != 0)
    //                            {
    //                                EditorGUI.BeginChangeCheck();
    //                                controllerVar.minShape = EditorGUILayout.FloatField(new GUIContent("Min" + ((double)controllerVar.minShape < 0.0 ? " (under)" : ""), "OFF value for the expression component -- usually 0.0f. NOTE: under-driving only supported in Unity 2018.3+ and also requires Project Settings > Player > Legacy blendshape clamp to be disabled."), controllerVar.minShape * 100f, new GUILayoutOption[0]) / 100f;
    //                                controllerVar.maxShape = EditorGUILayout.FloatField(new GUIContent("Max" + ((double)controllerVar.maxShape > 1.0 ? " (over)" : ""), "ON value for the expression component. NOTE: over-driving only supported in Unity 2018.3+ and also requires Project Settings > Player > Legacy blendshape clamp to be disabled."), controllerVar.maxShape * 100f, new GUILayoutOption[0]) / 100f;
    //                                if (EditorGUI.EndChangeCheck() && drawExpressionData.ExpData.previewDisplayMode)
    //                                {
    //                                    InspectorCommon.PreviewExpression(drawExpressionData.ExpData);
    //                                    break;
    //                                }
    //                                break;
    //                            }
    //                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                            GUILayout.Space(40f);
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //                            InspectorCommon.DrawStatusMessageBlock("Cannot adjust shape settings while Dynamics are enabled.", "Set dynamics to 1.0f to adjust shapes in preview mode.", Color.red);
    //                            GUILayout.EndHorizontal();
    //                            break;
    //                        }
    //                        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //                        InspectorCommon.DrawStatusMessageBlock(configState.msg, configState.detail, Color.red);
    //                        break;
    //                    case 1:
    //                        if (Object.op_Inequality((Object)controllerVar.bone, (Object)null))
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Normal);
    //                        else
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Error);
    //                        EditorGUI.BeginChangeCheck();
    //                        Transform bone1 = controllerVar.bone;
    //                        controllerVar.bone = (Transform)EditorGUILayout.ObjectField("Bone/Transform", (Object)controllerVar.bone, typeof(Transform), true, new GUILayoutOption[0]);
    //                        if (EditorGUI.EndChangeCheck())
    //                        {
    //                            if (drawExpressionData.ExpData.previewDisplayMode)
    //                            {
    //                                Transform bone2 = controllerVar.bone;
    //                                controllerVar.bone = bone1;
    //                                drawExpressionData.ExpData.previewDisplayMode = false;
    //                                InspectorCommon.PreviewExpression(drawExpressionData.ExpData);
    //                                controllerVar.bone = bone2;
    //                            }
    //                            controllerVar.ClearStoredTforms();
    //                            controllerVar.inspIsSetStart = false;
    //                            controllerVar.inspIsSetEnd = false;
    //                            if (Object.op_Implicit((Object)controllerVar.bone))
    //                            {
    //                                controllerVar.StoreBoneBase();
    //                                controllerVar.StoreStartTform();
    //                            }
    //                        }
    //                        InspectorCommon.DrawResetBg();
    //                        if (Object.op_Implicit((Object)controllerVar.bone))
    //                        {
    //                            if (drawExpressionData.InspDrawBoneCaptureControls)
    //                            {
    //                                if (((!drawExpressionData.ExpData.previewDisplayMode ? 0 : (Mathf.Approximately(drawExpressionData.Dynamics, 1f) ? 1 : 0)) | (!drawExpressionData.ExpData.previewDisplayMode ? 1 : 0)) != 0)
    //                                {
    //                                    InspectorCommon.DrawSeparationSpacing(5f);
    //                                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                                    GUILayout.Space(35f);
    //                                    if (components[index].enabled)
    //                                    {
    //                                        if (InspectorCommon.DrawItemButton("|<", "Click to reset the assigned transform to its original settings.", 20f))
    //                                            controllerVar.ResetBone(controllerVar.baseTform, true);
    //                                        if (InspectorCommon.isInspectorLocked)
    //                                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //                                        else
    //                                            InspectorCommon.DrawResetBg();
    //                                        if (InspectorCommon.DrawItemButton(InspectorCommon.isInspectorLocked ? "release" : "adjust", (InspectorCommon.isInspectorLocked ? "Unlock" : "Lock") + " the inspector and " + (InspectorCommon.isInspectorLocked ? "re-select the component object." : "select the bone to adjust."), 50f))
    //                                            InspectorCommon.SetInspectorLockAdjust(drawExpressionData.Go, ((Component)controllerVar.bone).gameObject);
    //                                    }
    //                                    else
    //                                        InspectorCommon.DrawItemNonButton("<>", "This component is disabled and cannot be adjusted.", 20f);
    //                                    InspectorCommon.DrawSeparationSpacing(20f);
    //                                    int num1;
    //                                    bool flag1 = (num1 = 1) != 0;
    //                                    bool flag2 = num1 != 0;
    //                                    bool flag3 = num1 != 0;
    //                                    int num2;
    //                                    bool flag4 = (num2 = 1) != 0;
    //                                    bool flag5 = num2 != 0;
    //                                    bool flag6 = num2 != 0;
    //                                    if (controllerVar.fracPos)
    //                                    {
    //                                        flag3 = Vector3.op_Equality(controllerVar.bone.localPosition, controllerVar.startTform.pos);
    //                                        flag6 = Vector3.op_Equality(controllerVar.bone.localPosition, controllerVar.endTform.pos);
    //                                    }
    //                                    if (controllerVar.fracRot)
    //                                    {
    //                                        flag2 = Quaternion.op_Equality(controllerVar.bone.localRotation, controllerVar.startTform.rot);
    //                                        flag5 = Quaternion.op_Equality(controllerVar.bone.localRotation, controllerVar.endTform.rot);
    //                                    }
    //                                    if (controllerVar.fracScl)
    //                                    {
    //                                        flag1 = Vector3.op_Equality(controllerVar.bone.localScale, controllerVar.startTform.scale);
    //                                        flag4 = Vector3.op_Equality(controllerVar.bone.localScale, controllerVar.endTform.scale);
    //                                    }
    //                                    bool flag7 = flag3 & flag2 & flag1 && controllerVar.inspIsSetStart;
    //                                    bool flag8 = flag6 & flag5 & flag4 && controllerVar.inspIsSetEnd;
    //                                    if (flag7 && controllerVar.inspIsSetStart)
    //                                        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Good);
    //                                    else if (controllerVar.inspIsSetStart)
    //                                        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Normal);
    //                                    else
    //                                        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Error);
    //                                    string text1 = "Min" + ((double)currentViewWidth > 340.0 ? (controllerVar.inspIsSetStart ? " is Set" : " not Set") : "") + (!flag7 || flag8 ? "" : "?");
    //                                    float width = (double)currentViewWidth < 340.0 ? 50f : 80f;
    //                                    if (!flag7 && components[index].enabled)
    //                                    {
    //                                        string tip = "Click to return to the captured minimum transform setting. This is similar to a blendshape's 0 setting.";
    //                                        if (InspectorCommon.DrawItemButton(text1, tip, width))
    //                                            controllerVar.ResetBone(controllerVar.startTform, false);
    //                                    }
    //                                    else
    //                                    {
    //                                        string tip = "This is the captured minimum transform setting. This is similar to a blendshape's 0 setting.";
    //                                        if (!components[index].enabled)
    //                                            tip = "This component is disabled and cannot be adjusted.";
    //                                        InspectorCommon.DrawItemNonButton(text1, tip, width);
    //                                    }
    //                                    InspectorCommon.DrawResetBg();
    //                                    if (!flag7 && !flag8 && components[index].enabled)
    //                                    {
    //                                        if (InspectorCommon.DrawItemButton("<", "Click to capture the current transform settings for the minimum (starting) position.", 20f))
    //                                        {
    //                                            if (InspectorCommon.isInspectorLocked)
    //                                                InspectorCommon.SetInspectorLockAdjust(drawExpressionData.Go, drawExpressionData.Go);
    //                                            controllerVar.StoreStartTform();
    //                                        }
    //                                    }
    //                                    else
    //                                        InspectorCommon.DrawSeparationSpacing(20f);
    //                                    InspectorCommon.DrawSeparationSpacing(10f);
    //                                    if (flag8)
    //                                        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Good);
    //                                    else if (controllerVar.inspIsSetEnd)
    //                                        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Normal);
    //                                    else
    //                                        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Error);
    //                                    string text2 = "Max" + ((double)currentViewWidth > 340.0 ? (controllerVar.inspIsSetEnd ? " is Set" : " not Set") : "") + (!flag8 || flag7 ? "" : "?");
    //                                    string tip1 = (flag8 ? "This is " : "Click to return to ") + "the captured maximum transform setting. This is similar to a blendshape's 100 setting.";
    //                                    if (!flag8 && controllerVar.inspIsSetEnd && components[index].enabled)
    //                                    {
    //                                        if (InspectorCommon.DrawItemButton(text2, tip1, width))
    //                                            controllerVar.ResetBone(controllerVar.endTform, false);
    //                                    }
    //                                    else
    //                                    {
    //                                        if (!controllerVar.inspIsSetEnd)
    //                                            tip1 = "The maximum setting has not been captured. Adjust the transform and click the 'set' button to capture a new position.";
    //                                        else if (!components[index].enabled)
    //                                            tip1 = "This component is disabled and cannot be adjusted.";
    //                                        InspectorCommon.DrawItemNonButton(text2, tip1, width);
    //                                    }
    //                                    InspectorCommon.DrawResetBg();
    //                                    if (!flag8 && !flag7)
    //                                    {
    //                                        if (InspectorCommon.DrawItemButton("<", "Click to capture the current transform settings for the maximum (ending) position.", 20f))
    //                                        {
    //                                            if (InspectorCommon.isInspectorLocked)
    //                                                InspectorCommon.SetInspectorLockAdjust(drawExpressionData.Go, drawExpressionData.Go);
    //                                            controllerVar.StoreEndTform();
    //                                        }
    //                                    }
    //                                    else
    //                                        InspectorCommon.DrawSeparationSpacing(20f);
    //                                    GUILayout.EndHorizontal();
    //                                    if (drawExpressionData.InspDrawConstraints)
    //                                    {
    //                                        InspectorCommon.DrawSeparationSpacing(10f);
    //                                        EditorGUILayout.LabelField("Transform Compute Constraints:", new GUILayoutOption[0]);
    //                                        ++EditorGUI.indentLevel;
    //                                        float labelWidth = EditorGUIUtility.labelWidth;
    //                                        float fieldWidth = EditorGUIUtility.fieldWidth;
    //                                        EditorGUIUtility.labelWidth = 20f;
    //                                        EditorGUIUtility.fieldWidth = 30f;
    //                                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                                        EditorGUILayout.LabelField(new GUIContent("Position", "Enable computation of position."), new GUILayoutOption[0]);
    //                                        EditorGUILayout.LabelField(new GUIContent("Rotation", "Enable computation of rotation."), new GUILayoutOption[0]);
    //                                        EditorGUILayout.LabelField(new GUIContent("Scale", "Enable computation of scale."), new GUILayoutOption[0]);
    //                                        InspectorCommon.DrawSeparationSpacing(50f);
    //                                        GUILayout.EndHorizontal();
    //                                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                                        controllerVar.fracPos = EditorGUILayout.Toggle(controllerVar.fracPos, new GUILayoutOption[0]);
    //                                        controllerVar.fracRot = EditorGUILayout.Toggle(controllerVar.fracRot, new GUILayoutOption[0]);
    //                                        controllerVar.fracScl = EditorGUILayout.Toggle(controllerVar.fracScl, new GUILayoutOption[0]);
    //                                        EditorGUIUtility.labelWidth = labelWidth;
    //                                        EditorGUIUtility.fieldWidth = fieldWidth;
    //                                        InspectorCommon.DrawSeparationSpacing(50f);
    //                                        GUILayout.EndHorizontal();
    //                                        --EditorGUI.indentLevel;
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                                    GUILayout.Space(40f);
    //                                    InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //                                    InspectorCommon.DrawStatusMessageBlock("Cannot adjust bone settings while Dynamics are enabled.", "Set dynamics to 1.0f to adjust bones in preview mode.", Color.red);
    //                                    GUILayout.EndHorizontal();
    //                                }
    //                            }
    //                            if (!configState.state)
    //                            {
    //                                InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //                                InspectorCommon.DrawStatusMessageBlock(configState.msg, configState.detail, Color.red);
    //                                break;
    //                            }
    //                            break;
    //                        }
    //                        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //                        InspectorCommon.DrawStatusMessageBlock("NOT READY: A bone Transform is needed.", "Link an appropriate Transform to properly configure this expression component.", Color.red);
    //                        break;
    //                    case 2:
    //                        InspectorCommon.DrawSwitcherController<Sprite>(ref controllerVar.sprites, controllerVar, (ExpressionComponent.ControlType)2, configState, drawExpressionData.ExpData, drawExpressionData.Dynamics);
    //                        break;
    //                    case 3:
    //                        InspectorCommon.DrawSwitcherController<Sprite>(ref controllerVar.sprites, controllerVar, (ExpressionComponent.ControlType)3, configState, drawExpressionData.ExpData, drawExpressionData.Dynamics);
    //                        break;
    //                    case 4:
    //                        InspectorCommon.DrawSwitcherController<Texture>(ref controllerVar.textures, controllerVar, (ExpressionComponent.ControlType)4, configState, drawExpressionData.ExpData, drawExpressionData.Dynamics);
    //                        break;
    //                    case 5:
    //                        InspectorCommon.DrawSwitcherController<Material>(ref controllerVar.materials, controllerVar, (ExpressionComponent.ControlType)5, configState, drawExpressionData.ExpData, drawExpressionData.Dynamics);
    //                        break;
    //                    case 6:
    //                        if (configState.state)
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Normal);
    //                        else
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Error);
    //                        UmaUepProxy umaUepProxy = controllerVar.umaUepProxy;
    //                        int blendIndex2 = controllerVar.blendIndex;
    //                        EditorGUI.BeginChangeCheck();
    //                        controllerVar.umaUepProxy = (UmaUepProxy)EditorGUILayout.ObjectField((double)currentViewWidth < 460.0 ? "ExprPlayer" : "UMA ExpressionPlayer", (Object)controllerVar.umaUepProxy, typeof(UmaUepProxy), true, new GUILayoutOption[0]);
    //                        if (EditorGUI.EndChangeCheck() && drawExpressionData.ExpData.previewDisplayMode)
    //                        {
    //                            umaUepProxy.SetPose(blendIndex2, 0.0f);
    //                            drawExpressionData.ExpData.previewDisplayMode = false;
    //                            InspectorCommon.PreviewExpression(drawExpressionData.ExpData);
    //                            drawExpressionData.ExpData.previewDisplayMode = true;
    //                        }
    //                        InspectorCommon.DrawResetBg();
    //                        if (configState.state && Object.op_Implicit((Object)controllerVar.umaUepProxy))
    //                        {
    //                            EditorGUI.BeginChangeCheck();
    //                            controllerVar.blendIndex = EditorGUILayout.Popup("Expression", controllerVar.blendIndex, controllerVar.umaUepProxy.GetPoseNames(), new GUILayoutOption[0]);
    //                            if (EditorGUI.EndChangeCheck() && drawExpressionData.ExpData.previewDisplayMode)
    //                            {
    //                                controllerVar.umaUepProxy.SetPose(blendIndex2, 0.0f);
    //                                InspectorCommon.PreviewExpression(drawExpressionData.ExpData);
    //                            }
    //                            if (controllerVar.blendIndex > controllerVar.umaUepProxy.Poses.Length)
    //                                controllerVar.blendIndex = 0;
    //                            if (((!drawExpressionData.ExpData.previewDisplayMode ? 0 : (Mathf.Approximately(drawExpressionData.Dynamics, 1f) ? 1 : 0)) | (!drawExpressionData.ExpData.previewDisplayMode ? 1 : 0)) != 0)
    //                            {
    //                                EditorGUI.BeginChangeCheck();
    //                                controllerVar.uepAmount = EditorGUILayout.Slider(new GUIContent("Amount", "Value for the expression component."), controllerVar.uepAmount, controllerVar.umaUepProxy.GetMode(controllerVar.blendIndex) ? -1f : 0.0f, 1f, new GUILayoutOption[0]);
    //                                if (EditorGUI.EndChangeCheck() && drawExpressionData.ExpData.previewDisplayMode)
    //                                {
    //                                    InspectorCommon.PreviewExpression(drawExpressionData.ExpData);
    //                                    InspectorCommon.TwistTheTwirler(drawExpressionData.Go);
    //                                    break;
    //                                }
    //                                break;
    //                            }
    //                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                            GUILayout.Space(40f);
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //                            InspectorCommon.DrawStatusMessageBlock("Cannot adjust shape settings while Dynamics are enabled.", "Set dynamics to 1.0f to adjust shapes in preview mode.", Color.red);
    //                            GUILayout.EndHorizontal();
    //                            break;
    //                        }
    //                        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //                        InspectorCommon.DrawStatusMessageBlock(configState.msg, configState.detail, Color.red);
    //                        break;
    //                    case 7:
    //                        if (Object.op_Implicit((Object)controllerVar.animator))
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Normal);
    //                        else
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Error);
    //                        EditorGUI.BeginChangeCheck();
    //                        controllerVar.animator = (Animator)EditorGUILayout.ObjectField((double)currentViewWidth < 490.0 ? "Animator" : "Unity Animator Component", (Object)controllerVar.animator, typeof(Animator), true, new GUILayoutOption[0]);
    //                        if (EditorGUI.EndChangeCheck())
    //                            controllerVar.blendIndex = 0;
    //                        InspectorCommon.DrawResetBg();
    //                        if (Object.op_Implicit((Object)controllerVar.animator))
    //                        {
    //                            if (!controllerVar.animator.isInitialized)
    //                                controllerVar.animator.Rebind();
    //                            if (Object.op_Inequality((Object)controllerVar.animator.runtimeAnimatorController, (Object)null) && controllerVar.animator.parameterCount > 0)
    //                            {
    //                                controllerVar.blendIndex = EditorGUILayout.Popup((double)currentViewWidth < 410.0 ? "Parameter" : "Animator Parameter", controllerVar.blendIndex, SalsaUtil.GetAnimatorParameters(controllerVar.animator), new GUILayoutOption[0]);
    //                                string str = controllerVar.animator.GetParameter(controllerVar.blendIndex).type.ToString();
    //                                if (str == "Trigger")
    //                                {
    //                                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                                    GUILayout.FlexibleSpace();
    //                                    controllerVar.isTriggerParameterBiDirectional = GUILayout.Toggle(controllerVar.isTriggerParameterBiDirectional, new GUIContent("Trigger Both Ways?", "Trigger paramaters only trigger when ON phase begins by default. Enable to also trigger when OFF phase begins."), new GUILayoutOption[0]);
    //                                    GUILayout.EndHorizontal();
    //                                }
    //                                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                                GUILayout.FlexibleSpace();
    //                                InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Normal);
    //                                string tip = "";
    //                                if (!(str == "Trigger"))
    //                                {
    //                                    if (!(str == "Float"))
    //                                    {
    //                                        if (!(str == "Int"))
    //                                        {
    //                                            if (str == "Bool")
    //                                                tip = "Bool is set true on animation ON start and false on animation OFF start.";
    //                                        }
    //                                        else
    //                                            tip = "Int parameter is set to 1 on animation ON finish and is set to 0 on animation OFF finish.";
    //                                    }
    //                                    else
    //                                        tip = "Float parameter is set throughout the expression animation cycle as [0.0f <> 1.0f].";
    //                                }
    //                                else
    //                                    tip = "Trigger is set on animation ON start and reset on animation ON finish. (Optionally) sent on animation OFF start and reset on animation OFF finish.";
    //                                InspectorCommon.DrawStatusMessageBlock(str + " parameter selected.", tip, InspectorCommon.inspectorTextColor);
    //                                InspectorCommon.DrawResetBg();
    //                                GUILayout.EndHorizontal();
    //                            }
    //                        }
    //                        if (!configState.state)
    //                        {
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //                            InspectorCommon.DrawStatusMessageBlock(configState.msg, configState.detail, Color.red);
    //                            break;
    //                        }
    //                        break;
    //                    case 8:
    //                        if (configState.state)
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Normal);
    //                        else
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Error);
    //                        controllerVar.eventIdentityName = EditorGUILayout.TextField(new GUIContent("Event Identifier", "Enter a name (case-sensitive) to identify this event to subscribers."), controllerVar.eventIdentityName, new GUILayoutOption[0]);
    //                        InspectorCommon.DrawResetBg();
    //                        if (!configState.state || string.IsNullOrEmpty(controllerVar.eventIdentityName))
    //                        {
    //                            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //                            InspectorCommon.DrawStatusMessageBlock("A unique identifying name is required.", "Subscribers need an identifying element to filter events from different controllers.", Color.red);
    //                            break;
    //                        }
    //                        break;
    //                }
    //                controllerVar.RemoveUnusedLists(components[index].controlType);
    //                InspectorCommon.DrawResetBg();
    //                --EditorGUI.indentLevel;
    //                InspectorCommon.DrawSeparationSpacing(5f);
    //            }
    //            GUILayout.EndVertical();
    //            InspectorCommon.DrawResetBg();
    //            InspectorCommon.DrawSeparationSpacing(5f);
    //        }
    //    }
    //    InspectorCommon.DrawResetBg();
    //}

    //private static void DrawAnimatorControlledOption(
    //  List<ExpressionComponent> components,
    //  int cmpnt,
    //  bool drawOffsetOptions)
    //{
    //    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //    GUILayout.Space(15f);
    //    components[cmpnt].isAnimatorControlled = GUILayout.Toggle(components[cmpnt].isAnimatorControlled, new GUIContent("Use Animator Controlled MergeBack?", "Enable if an external process is also controlling this transform (i.e. Mecanim), enable this option to smoothly return to external control."), new GUILayoutOption[0]);
    //    GUILayout.EndHorizontal();
    //    if (components[cmpnt].controlType == 1 & drawOffsetOptions)
    //    {
    //        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //        GUILayout.Space(15f);
    //        components[cmpnt].useOffset = GUILayout.Toggle(components[cmpnt].useOffset, new GUIContent("Use Offset?", "Applies a relative offset to the Animator, instead of an absolute calculation."), new GUILayoutOption[0]);
    //        GUILayout.EndHorizontal();
    //        if (components[cmpnt].useOffset)
    //        {
    //            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //            GUILayout.Space(35f);
    //            components[cmpnt].useOffsetFollow = GUILayout.Toggle(components[cmpnt].useOffsetFollow, new GUIContent("Follow?", "Continuously applies a relative offset to the Animator, following the influence instead of a singlular, starting calculation."), new GUILayoutOption[0]);
    //            GUILayout.EndHorizontal();
    //        }
    //    }
    //    InspectorCommon.DrawSeparationSpacing(5f);
    //}

    //public static void TwistTheTwirler(GameObject go)
    //{
    //    UmaUepEditorPreviewTwirler componentInChildren = ((Component)go.transform).GetComponentInChildren<UmaUepEditorPreviewTwirler>();
    //    if (!Object.op_Inequality((Object)componentInChildren, (Object)null))
    //        return;
    //    ((Component)componentInChildren).transform.localRotation = Random.rotation;
    //}

    //private static void DrawSwitcherController<T>(
    //  ref List<T> elementList,
    //  InspectorControllerHelperData helperData,
    //  ExpressionComponent.ControlType controlType,
    //  ExpressionConfigurationState configState,
    //  Expression expression,
    //  float dyanmics)
    //  where T : Object
    //{
    //    bool flag = false;
    //    string name = typeof(T).Name;
    //    if (elementList == null)
    //        elementList = new List<T>();
    //    switch (controlType - 2)
    //    {
    //        case 0:
    //            flag = Object.op_Inequality((Object)helperData.spriteRenderer, (Object)null);
    //            InspectorCommon.DrawBackgroundCondition(flag ? InspectorCommon.AlertType.Normal : InspectorCommon.AlertType.Error);
    //            helperData.spriteRenderer = (SpriteRenderer)EditorGUILayout.ObjectField("Renderer", (Object)helperData.spriteRenderer, typeof(SpriteRenderer), true, new GUILayoutOption[0]);
    //            break;
    //        case 1:
    //            flag = Object.op_Inequality((Object)helperData.uguiRenderer, (Object)null);
    //            InspectorCommon.DrawBackgroundCondition(flag ? InspectorCommon.AlertType.Normal : InspectorCommon.AlertType.Error);
    //            helperData.uguiRenderer = (Image)EditorGUILayout.ObjectField("Renderer", (Object)helperData.uguiRenderer, typeof(Image), true, new GUILayoutOption[0]);
    //            break;
    //        case 2:
    //            flag = Object.op_Inequality((Object)helperData.textureRenderer, (Object)null);
    //            InspectorCommon.DrawBackgroundCondition(flag ? InspectorCommon.AlertType.Normal : InspectorCommon.AlertType.Error);
    //            Renderer textureRenderer = helperData.textureRenderer;
    //            EditorGUI.BeginChangeCheck();
    //            helperData.textureRenderer = (Renderer)EditorGUILayout.ObjectField("Renderer", (Object)helperData.textureRenderer, typeof(Renderer), true, new GUILayoutOption[0]);
    //            if (EditorGUI.EndChangeCheck())
    //            {
    //                if (Object.op_Inequality((Object)textureRenderer, (Object)null) && Object.op_Inequality((Object)textureRenderer, (Object)helperData.textureRenderer))
    //                {
    //                    if (expression.previewDisplayMode)
    //                    {
    //                        expression.previewDisplayMode = false;
    //                        InspectorCommon.PreviewExpression(expression);
    //                    }
    //                    for (int index = 0; index < textureRenderer.sharedMaterials.Length; ++index)
    //                        textureRenderer.sharedMaterials[index].SetTexture(SalsaSettings.RenderPipelineTextureName, helperData.backupTextures[index]);
    //                }
    //                helperData.backupTextures.Clear();
    //                if (Object.op_Implicit((Object)helperData.textureRenderer))
    //                {
    //                    foreach (Material sharedMaterial in helperData.textureRenderer.sharedMaterials)
    //                        helperData.backupTextures.Add(sharedMaterial.GetTexture(SalsaSettings.RenderPipelineTextureName));
    //                }
    //            }
    //            if (Object.op_Implicit((Object)helperData.textureRenderer) && helperData.textureRenderer.sharedMaterials.Length > 1)
    //            {
    //                InspectorCommon.DrawResetBg();
    //                string[] strArray = new string[helperData.textureRenderer.sharedMaterials.Length];
    //                for (int index = 0; index < helperData.textureRenderer.sharedMaterials.Length; ++index)
    //                    strArray[index] = ((Object)helperData.textureRenderer.sharedMaterials[index]).name;
    //                ++EditorGUI.indentLevel;
    //                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                helperData.materialIndex = EditorGUILayout.Popup("Material Selection", helperData.materialIndex, strArray, new GUILayoutOption[0]);
    //                if (!expression.previewDisplayMode && InspectorCommon.DrawItemButton("|<", "Restore original textures to material."))
    //                    helperData.textureRenderer.sharedMaterials[helperData.materialIndex].SetTexture(SalsaSettings.RenderPipelineTextureName, helperData.backupTextures[helperData.materialIndex]);
    //                EditorGUILayout.EndHorizontal();
    //                --EditorGUI.indentLevel;
    //                break;
    //            }
    //            helperData.materialIndex = 0;
    //            break;
    //        case 3:
    //            flag = Object.op_Inequality((Object)helperData.materialRenderer, (Object)null);
    //            InspectorCommon.DrawBackgroundCondition(flag ? InspectorCommon.AlertType.Normal : InspectorCommon.AlertType.Error);
    //            helperData.materialRenderer = (Renderer)EditorGUILayout.ObjectField("Renderer", (Object)helperData.materialRenderer, typeof(Renderer), true, new GUILayoutOption[0]);
    //            break;
    //    }
    //    InspectorCommon.DrawResetBg();
    //    if (flag)
    //    {
    //        helperData.onState = (Switcher.OnState)EditorGUILayout.EnumPopup(new GUIContent("On State", "Select the appropriate processing mechanism to control the on/off state of the " + name.ToLower() + "."), (Enum)(object)helperData.onState, new GUILayoutOption[0]);
    //        InspectorCommon.DrawSeparationSpacing(5f);
    //        GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
    //        string[] strArray = InspectorCommon.CheckDragDrop<T>(elementList);
    //        if (strArray != null)
    //        {
    //            foreach (string str in strArray)
    //            {
    //                T obj = AssetDatabase.LoadAssetAtPath<T>(str);
    //                if (!Object.op_Equality((Object)obj, (Object)null))
    //                    elementList.Add(obj);
    //            }
    //            if (!expression.previewDisplayMode)
    //                InspectorCommon.Update2dRendererElement<T>(elementList, helperData, controlType);
    //        }
    //        if (expression.previewDisplayMode)
    //            InspectorCommon.PreviewExpression(expression, dyanmics);
    //        if (elementList.Count == 0)
    //        {
    //            InspectorCommon.DrawSeparationSpacing(5f);
    //            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //            GUILayout.FlexibleSpace();
    //            if (InspectorCommon.DrawItemButton("+", "Manually add reference slots for 2D elements."))
    //                elementList.Add(default(T));
    //            GUILayout.EndHorizontal();
    //        }
    //        if (elementList.Count > 0 && (Event.current.type != 10 || strArray == null))
    //        {
    //            InspectorCommon.DrawSeparationSpacing(5f);
    //            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //            helperData.display2dImage = GUILayout.Toggle(helperData.display2dImage, new GUIContent("Display " + name + " Elements", "Enable to toggle the inspector to display the " + name.ToLower() + " elements."), new GUILayoutOption[0]);
    //            GUILayout.FlexibleSpace();
    //            EditorGUI.BeginChangeCheck();
    //            helperData.isRestNull = GUILayout.Toggle(helperData.isRestNull, new GUIContent("null Rest", "Sets the 'Rest/Off' sprite as null. This is useful if it is desirable to not have a " + name.ToLower() + " display in the off/rest state."), new GUILayoutOption[0]);
    //            if (EditorGUI.EndChangeCheck() && !expression.previewDisplayMode)
    //                InspectorCommon.Update2dRendererElement<T>(elementList, helperData, controlType);
    //            GUILayout.Space(10f);
    //            if (InspectorCommon.DrawItemButton("delete all", "Delete all 2D elements from this expression component.", 65f))
    //            {
    //                elementList.Clear();
    //                helperData.isRestNull = false;
    //            }
    //            if (InspectorCommon.DrawItemButton("+", "Manually add reference slots for 2D elements."))
    //                elementList.Add(default(T));
    //            GUILayout.EndHorizontal();
    //            InspectorCommon.DrawSeparationSpacing(5f);
    //            if (helperData.isRestNull)
    //                EditorGUILayout.LabelField(new GUIContent(name + " REST is NULL", "The Rest/Off state is set to null and will present nothing when the animation is off."), new GUILayoutOption[0]);
    //            int num = helperData.display2dImage ? 72 : 16;
    //            for (int index = 0; index < elementList.Count; ++index)
    //            {
    //                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //                EditorGUI.BeginChangeCheck();
    //                elementList[index] = (T)EditorGUILayout.ObjectField(name + index.ToString() + (index != 0 || helperData.isRestNull ? (index == elementList.Count - 1 ? " MAX" : "") : " REST"), (Object)elementList[index], typeof(T), true, new GUILayoutOption[1]
    //                {
    //          GUILayout.Height((float) num)
    //                });
    //                if (EditorGUI.EndChangeCheck() && !helperData.isRestNull)
    //                {
    //                    switch (controlType - 2)
    //                    {
    //                        case 0:
    //                            helperData.spriteRenderer.sprite = (object)elementList[0] as Sprite;
    //                            break;
    //                        case 1:
    //                            helperData.uguiRenderer.sprite = (object)elementList[0] as Sprite;
    //                            break;
    //                        case 2:
    //                            helperData.textureRenderer.sharedMaterials[helperData.materialIndex].SetTexture(SalsaSettings.RenderPipelineTextureName, (object)elementList[0] as Texture);
    //                            break;
    //                        case 3:
    //                            helperData.materialRenderer.sharedMaterial = (object)elementList[0] as Material;
    //                            break;
    //                    }
    //                }
    //                if (InspectorCommon.DrawListItemSortable<T>(elementList, index) && !expression.previewDisplayMode)
    //                    InspectorCommon.Update2dRendererElement<T>(elementList, helperData, controlType);
    //                GUILayout.EndHorizontal();
    //            }
    //        }
    //        if (!configState.state)
    //        {
    //            InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //            InspectorCommon.DrawStatusMessageBlock(configState.msg, configState.detail, Color.red);
    //        }
    //        GUILayout.EndVertical();
    //    }
    //    else
    //    {
    //        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Warning);
    //        InspectorCommon.DrawStatusMessageBlock("NOT READY: Link an appropriate controller.", "Link a Renderer to properly configure this expression component.", Color.red);
    //    }
    //}

    //private static void Update2dRendererElement<T>(
    //  List<T> elementList,
    //  InspectorControllerHelperData helperData,
    //  ExpressionComponent.ControlType controlType)
    //{
    //    switch (controlType - 2)
    //    {
    //        case 0:
    //            helperData.spriteRenderer.sprite = helperData.isRestNull | elementList.Count == 0 ? (Sprite)null : (object)elementList[0] as Sprite;
    //            break;
    //        case 1:
    //            helperData.uguiRenderer.sprite = helperData.isRestNull | elementList.Count == 0 ? (Sprite)null : (object)elementList[0] as Sprite;
    //            break;
    //        case 2:
    //            if (Object.op_Equality((Object)helperData.textureRenderer.sharedMaterials[helperData.materialIndex], (Object)null))
    //            {
    //                Debug.LogWarning((object)("There is no material assigned to the linked renderer " + ((Object)helperData.textureRenderer).name + ", instanceID: " + ((Object)helperData.textureRenderer).GetInstanceID().ToString() + ". Ensure a material is applied to this renderer to prevent additional runtime errors."));
    //                break;
    //            }
    //            helperData.textureRenderer.sharedMaterials[helperData.materialIndex].SetTexture(SalsaSettings.RenderPipelineTextureName, helperData.isRestNull | elementList.Count == 0 ? (Texture)null : (object)elementList[0] as Texture);
    //            break;
    //        case 3:
    //            helperData.materialRenderer.sharedMaterial = helperData.isRestNull | elementList.Count == 0 ? (Material)null : (object)elementList[0] as Material;
    //            break;
    //    }
    //}

    //public static void SetInspectorLockAdjust(GameObject goComponent, GameObject goAdjustable)
    //{
    //    InspectorCommon.isInspectorLocked = !InspectorCommon.isInspectorLocked;
    //    ActiveEditorTracker.sharedTracker.isLocked = InspectorCommon.isInspectorLocked;
    //    if (InspectorCommon.isInspectorLocked)
    //        Selection.SetActiveObjectWithContext((Object)goAdjustable, (Object)goComponent);
    //    else
    //        Selection.SetActiveObjectWithContext((Object)goComponent, (Object)goComponent);
    //}

    //public static void DrawAnimationTimings(
    //  bool drawTimingHold,
    //  bool drawTimingOff,
    //  bool drawTimingDelay,
    //  ExpressionComponent.ExpressionType type,
    //  ref float durDelay,
    //  ref float durON,
    //  ref float durHOLD,
    //  ref float durOFF,
    //  byte overridesFlags)
    //{
    //    InspectorCommon.DrawSeparationSpacing(5f);
    //    if (((int)overridesFlags & 1) == 0)
    //    {
    //        ++EditorGUI.indentLevel;
    //        EditorGUILayout.LabelField("Animation Timings (seconds):", new GUILayoutOption[0]);
    //        ++EditorGUI.indentLevel;
    //        float labelWidth = EditorGUIUtility.labelWidth;
    //        float fieldWidth = EditorGUIUtility.fieldWidth;
    //        EditorGUIUtility.labelWidth = 20f;
    //        EditorGUIUtility.fieldWidth = 5f;
    //        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //        EditorGUILayout.LabelField(new GUIContent("On", "Time in seconds to animate to the ON extent."), new GUILayoutOption[0]);
    //        if (drawTimingHold)
    //            EditorGUILayout.LabelField(new GUIContent("Hold", "Time in seconds to hold the ON extent before animating OFF."), new GUILayoutOption[0]);
    //        if (drawTimingOff)
    //            EditorGUILayout.LabelField(new GUIContent("Off", "Time in seconds to animate to the OFF extent."), new GUILayoutOption[0]);
    //        InspectorCommon.DrawSeparationSpacing(10f);
    //        GUILayout.EndHorizontal();
    //        EditorGUIUtility.labelWidth = 5f;
    //        EditorGUIUtility.fieldWidth = 20f;
    //        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //        durON = EditorGUILayout.FloatField(Mathf.Clamp(durON, 0.0f, 100f), new GUILayoutOption[0]);
    //        if (drawTimingHold)
    //            durHOLD = EditorGUILayout.FloatField(Mathf.Clamp(durHOLD, 0.0f, 100f), new GUILayoutOption[0]);
    //        if (drawTimingOff)
    //            durOFF = EditorGUILayout.FloatField(Mathf.Clamp(durOFF, 0.0f, 100f), new GUILayoutOption[0]);
    //        InspectorCommon.DrawSeparationSpacing(10f);
    //        GUILayout.EndHorizontal();
    //        if (drawTimingDelay)
    //        {
    //            GUILayout.Space(5f);
    //            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
    //            EditorGUIUtility.labelWidth = 80f;
    //            EditorGUIUtility.fieldWidth = 30f;
    //            durDelay = EditorGUILayout.FloatField(new GUIContent("Delay", "Enables a delay to the start of the animation timings cycles. Duration ON will begin after this time has expired."), Mathf.Max(durDelay, 0.0f), new GUILayoutOption[0]);
    //            GUILayout.FlexibleSpace();
    //            GUILayout.EndHorizontal();
    //        }
    //        EditorGUIUtility.labelWidth = labelWidth;
    //        EditorGUIUtility.fieldWidth = fieldWidth;
    //    }
    //    else
    //        InspectorCommon.DrawSettingsOverridden("Timings");
    //    EditorGUI.indentLevel = 0;
    //}

    private static void DrawSettingsOverridden(string overriddenElement)
    {
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        InspectorCommon.DrawBackgroundCondition(InspectorCommon.AlertType.Normal);
        GUILayout.Space(15f);
        InspectorCommon.DrawStatusMessageBlock("NOTE: " + overriddenElement + " have been overridden.", overriddenElement + " for this component have been overridden at a higher level.", Color.black);
        InspectorCommon.DrawResetBg();
        GUILayout.EndHorizontal();
    }

    private static bool DrawListItemSortable<T>(List<T> list, int i)
    {
        bool flag1 = i == 0;
        bool flag2 = i == list.Count - 1;
        if (InspectorCommon.DrawItemButton(flag1 ? "?" : "?", (flag1 ? "Cannot " : "") + "Move Up", 20f) && !flag1)
        {
            list.Insert(i - 1, list[i]);
            list.RemoveAt(i + 1);
            return true;
        }
        if (InspectorCommon.DrawItemButton(flag2 ? "?" : "?", (flag2 ? "Cannot " : "") + "Move Down", 20f) && !flag2)
        {
            list.Insert(i, list[i + 1]);
            list.RemoveAt(i + 2);
            return true;
        }
        if (!InspectorCommon.DrawItemButton("x", "Delete Item", 20f))
            return false;
        list.RemoveAt(i);
        return true;
    }

    //private static string[] CheckDragDrop<T>(List<T> list)
    //{
    //    Rect rect = GUILayoutUtility.GetRect(0.0f, 25f, new GUILayoutOption[1]
    //    {
    //    GUILayout.ExpandWidth(true)
    //    });
    //    GUI.backgroundColor = list.Count != 0 ? InspectorCommon.ClrBgGood : InspectorCommon.ClrBgError;
    //    GUI.Box(rect, "Drag and Drop 2D Elements Here", InspectorCommon.StyleBoldCentered);
    //    InspectorCommon.DrawResetBg();
    //    Event current = Event.current;
    //    if (((Rect)ref rect).Contains(current.mousePosition))
    //    {
    //        EventType type = current.type;
    //        if (type != 9)
    //        {
    //            if (type == 10)
    //            {
    //                DragAndDrop.visualMode = (DragAndDropVisualMode)1;
    //                if (current.type == 10)
    //                {
    //                    DragAndDrop.AcceptDrag();
    //                    return DragAndDrop.paths;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            bool flag = false;
    //            foreach (string path in DragAndDrop.paths)
    //            {
    //                if (Object.op_Equality(AssetDatabase.LoadAssetAtPath(path, typeof(T)), (Object)null))
    //                {
    //                    flag = true;
    //                    break;
    //                }
    //            }
    //            DragAndDrop.visualMode = !flag ? (DragAndDropVisualMode)1 : (DragAndDropVisualMode)32;
    //        }
    //    }
    //    return (string[])null;
    //}

    //public static void AttachToLocalControllerComponent(
    //  ExpressionComponent.ControlType controlType,
    //  InspectorControllerHelperData controlHelper,
    //  GameObject go)
    //{
    //    if (controlType != null && Object.op_Inequality((Object)controlHelper.eyeGizmo, (Object)null))
    //        Object.DestroyImmediate((Object)((Component)controlHelper.eyeGizmo).gameObject);
    //    switch ((int)controlType)
    //    {
    //        case 0:
    //            controlHelper.smr = go.GetComponent<SkinnedMeshRenderer>();
    //            controlHelper.eyeGizmo = go.GetComponent<EyeGizmo>();
    //            break;
    //        case 2:
    //            controlHelper.spriteRenderer = go.GetComponent<SpriteRenderer>();
    //            break;
    //        case 3:
    //            controlHelper.uguiRenderer = go.GetComponent<Image>();
    //            break;
    //        case 4:
    //            controlHelper.textureRenderer = (Renderer)go.GetComponent<MeshRenderer>();
    //            if (!Object.op_Implicit((Object)controlHelper.textureRenderer))
    //                break;
    //            controlHelper.backupTextures.Clear();
    //            foreach (Material sharedMaterial in controlHelper.textureRenderer.sharedMaterials)
    //                controlHelper.backupTextures.Add(sharedMaterial.GetTexture(SalsaSettings.RenderPipelineTextureName));
    //            break;
    //        case 5:
    //            controlHelper.materialRenderer = (Renderer)go.GetComponent<MeshRenderer>();
    //            break;
    //        case 6:
    //            controlHelper.umaUepProxy = go.GetComponent<UmaUepProxy>();
    //            break;
    //        case 7:
    //            controlHelper.animator = go.GetComponent<Animator>();
    //            break;
    //        case 8:
    //            controlHelper.eventSender = go;
    //            break;
    //    }
    //}

    //public static ExpressionConfigurationState ConfigurationCheckExpression(
    //  Expression expData,
    //  bool drawBoneCaptureControls)
    //{
    //    if (expData.components.Count == 0)
    //        return new ExpressionConfigurationState(false, "At least one (1) expression component is required.", "Add expression components to form an overall expression. There must be at least one expression component on each expression.");
    //    for (int i = 0; i < expData.controllerVars.Count; ++i)
    //    {
    //        ExpressionConfigurationState configurationState = InspectorCommon.ConfigurationCheckExpressionComponent(expData, i, drawBoneCaptureControls);
    //        if (!configurationState.state)
    //            return configurationState;
    //    }
    //    return new ExpressionConfigurationState(true, "", "");
    //}

    //private static ExpressionConfigurationState ConfigurationCheckExpressionComponent(
    //  Expression expData,
    //  int i,
    //  bool drawBoneCaptureControls)
    //{
    //    ExpressionConfigurationState configurationState = new ExpressionConfigurationState();
    //    configurationState.state = true;
    //    configurationState.msg = "";
    //    configurationState.detail = "";
    //    if (!expData.controllerVars[i].HasLinkedController(expData.components[i].controlType))
    //    {
    //        configurationState.state = false;
    //        configurationState.msg = "NOT READY: Link an appropriate controller.";
    //        configurationState.detail = "A valid controller type is necessary. Once linked, additional options/settings will be available.";
    //        return configurationState;
    //    }
    //    bool flag = false;
    //    switch ((int)expData.components[i].controlType)
    //    {
    //        case 0:
    //            if (SalsaUtil.GetBlendShapes(expData.controllerVars[i].smr).Length == 0)
    //            {
    //                configurationState.state = false;
    //                configurationState.msg = "NOT READY: No blendshapes found.";
    //                configurationState.detail = "The SkinnedMeshRenderer does not have blendshapes. Ensure the model was exported with blendshapes attached.";
    //                break;
    //            }
    //            break;
    //        case 1:
    //            if (drawBoneCaptureControls && (!expData.controllerVars[i].inspIsSetEnd || !expData.controllerVars[i].inspIsSetStart))
    //            {
    //                configurationState.state = false;
    //                configurationState.msg = "NOT READY: The 'end' position has not been captured.";
    //                configurationState.detail = "Manipulate the object's transform gizmos to the appropriate settings and then click 'Capture End' to store the settings.";
    //                break;
    //            }
    //            break;
    //        case 2:
    //        case 3:
    //            if (expData.controllerVars[i].sprites == null || ((expData.controllerVars[i].sprites.Count == 0 ? 1 : 0) | (expData.controllerVars[i].sprites.Count != 1 ? 0 : (!expData.controllerVars[i].isRestNull ? 1 : 0))) != 0)
    //            {
    //                flag = true;
    //                break;
    //            }
    //            break;
    //        case 4:
    //            if (expData.controllerVars[i].textures == null || ((expData.controllerVars[i].textures.Count == 0 ? 1 : 0) | (expData.controllerVars[i].textures.Count != 1 ? 0 : (!expData.controllerVars[i].isRestNull ? 1 : 0))) != 0)
    //            {
    //                flag = true;
    //                break;
    //            }
    //            break;
    //        case 5:
    //            if (expData.controllerVars[i].materials == null || ((expData.controllerVars[i].materials.Count == 0 ? 1 : 0) | (expData.controllerVars[i].materials.Count != 1 ? 0 : (!expData.controllerVars[i].isRestNull ? 1 : 0))) != 0)
    //            {
    //                flag = true;
    //                break;
    //            }
    //            break;
    //        case 7:
    //            Animator animator = expData.controllerVars[i].animator;
    //            if (!animator.isInitialized)
    //                animator.Rebind();
    //            if (Object.op_Inequality((Object)animator, (Object)null) && ((Behaviour)animator).isActiveAndEnabled && Object.op_Inequality((Object)animator.runtimeAnimatorController, (Object)null))
    //            {
    //                if (animator.parameterCount == 0)
    //                {
    //                    configurationState.state = false;
    //                    configurationState.msg = "NOT READY: There are no parameters configured on this Animator.";
    //                    configurationState.detail = "Configure an appropriate parameter on the linked Animator for this ExpressionController to control.";
    //                    break;
    //                }
    //                break;
    //            }
    //            configurationState.state = false;
    //            configurationState.msg = "NOT READY: Animator is not active/enabled or does not have an attached RuntimeAnimationController.";
    //            configurationState.detail = "A valid Animator controller type is necessary. Once linked, additional options/settings will be available.";
    //            break;
    //    }
    //    if (flag)
    //    {
    //        configurationState.state = false;
    //        configurationState.msg = "A minimum of two (2) elements must be configured.";
    //        configurationState.detail = "At a minimum, the Off/Rest and On/Max elements must be set up here for a proper configuration. Off/Rest may be configured as null.";
    //    }
    //    return configurationState;
    //}

    //private static bool RemoveExpressionComponent(Expression expData, int componentIndex)
    //{
    //    bool previewDisplayMode = expData.previewDisplayMode;
    //    if (previewDisplayMode)
    //    {
    //        expData.previewDisplayMode = false;
    //        InspectorCommon.PreviewExpression(expData);
    //    }
    //    if (Object.op_Inequality((Object)expData.controllerVars[componentIndex].eyeGizmo, (Object)null))
    //        Object.DestroyImmediate((Object)((Component)expData.controllerVars[componentIndex].eyeGizmo).gameObject);
    //    expData.components.RemoveAt(componentIndex);
    //    expData.controllerVars.RemoveAt(componentIndex);
    //    expData.previewDisplayMode = previewDisplayMode;
    //    return expData.components.Count == componentIndex;
    //}

    //public static void PreviewExpression(Expression expData, float frac = 1f, bool resetBone = false)
    //{
    //    for (int index1 = 0; index1 < expData.controllerVars.Count; ++index1)
    //    {
    //        switch ((int)expData.components[index1].controlType)
    //        {
    //            case 0:
    //                expData.controllerVars[index1].smr.SetBlendShapeWeight(expData.controllerVars[index1].blendIndex, !expData.previewDisplayMode || !expData.components[index1].enabled ? 0.0f : expData.controllerVars[index1].maxShape * 100f * frac);
    //                break;
    //            case 1:
    //                if (!expData.previewDisplayMode || !expData.components[index1].isBonePreviewUpdated || !expData.components[index1].enabled || resetBone)
    //                {
    //                    if (expData.controllerVars[index1].fracPos)
    //                        expData.controllerVars[index1].bone.localPosition = !expData.previewDisplayMode || !expData.components[index1].enabled ? expData.controllerVars[index1].baseTform.pos : expData.controllerVars[index1].endTform.pos;
    //                    if (expData.controllerVars[index1].fracRot)
    //                        expData.controllerVars[index1].bone.localRotation = !expData.previewDisplayMode || !expData.components[index1].enabled ? expData.controllerVars[index1].startTform.rot : Quaternion.Lerp(expData.controllerVars[index1].startTform.rot, expData.controllerVars[index1].endTform.rot, frac);
    //                    if (expData.controllerVars[index1].fracScl)
    //                        expData.controllerVars[index1].bone.localScale = !expData.previewDisplayMode || !expData.components[index1].enabled ? expData.controllerVars[index1].baseTform.scale : expData.controllerVars[index1].endTform.scale;
    //                    expData.components[index1].isBonePreviewUpdated = expData.previewDisplayMode && expData.components[index1].enabled;
    //                    break;
    //                }
    //                break;
    //            case 2:
    //                int index2 = Mathf.RoundToInt(Mathf.Lerp(0.0f, (float)(expData.controllerVars[index1].sprites.Count - 1), frac));
    //                expData.controllerVars[index1].spriteRenderer.sprite = !expData.previewDisplayMode || !expData.components[index1].enabled ? (expData.controllerVars[index1].isRestNull ? (Sprite)null : expData.controllerVars[index1].sprites[0]) : expData.controllerVars[index1].sprites[index2];
    //                break;
    //            case 3:
    //                int index3 = Mathf.RoundToInt(Mathf.Lerp(0.0f, (float)(expData.controllerVars[index1].sprites.Count - 1), frac));
    //                expData.controllerVars[index1].uguiRenderer.sprite = !expData.previewDisplayMode || !expData.components[index1].enabled ? (expData.controllerVars[index1].isRestNull ? (Sprite)null : expData.controllerVars[index1].sprites[0]) : expData.controllerVars[index1].sprites[index3];
    //                break;
    //            case 4:
    //                int index4 = Mathf.RoundToInt(Mathf.Lerp(0.0f, (float)(expData.controllerVars[index1].textures.Count - 1), frac));
    //                if (Object.op_Implicit((Object)expData.controllerVars[index1].textureRenderer))
    //                {
    //                    expData.controllerVars[index1].textureRenderer.sharedMaterials[expData.controllerVars[index1].materialIndex].SetTexture(SalsaSettings.RenderPipelineTextureName, !expData.previewDisplayMode || !expData.components[index1].enabled ? (expData.controllerVars[index1].isRestNull ? (Texture)null : expData.controllerVars[index1].textures[0]) : expData.controllerVars[index1].textures[index4]);
    //                    break;
    //                }
    //                break;
    //            case 5:
    //                int index5 = Mathf.RoundToInt(Mathf.Lerp(0.0f, (float)(expData.controllerVars[index1].materials.Count - 1), frac));
    //                expData.controllerVars[index1].materialRenderer.sharedMaterial = !expData.previewDisplayMode || !expData.components[index1].enabled ? (expData.controllerVars[index1].isRestNull ? (Material)null : expData.controllerVars[index1].materials[0]) : expData.controllerVars[index1].materials[index5];
    //                break;
    //            case 6:
    //                expData.controllerVars[index1].umaUepProxy.SetPose(expData.controllerVars[index1].blendIndex, !expData.previewDisplayMode || !expData.components[index1].enabled ? 0.0f : expData.controllerVars[index1].uepAmount * frac);
    //                if (expData.previewDisplayMode)
    //                {
    //                    expData.controllerVars[index1].umaUepProxy.isPreviewing = true;
    //                    InspectorCommon.TwistTheTwirler(((Component)expData.controllerVars[index1].umaUepProxy).gameObject);
    //                    break;
    //                }
    //                expData.controllerVars[index1].umaUepProxy.isPreviewing = false;
    //                break;
    //        }
    //    }
    //}

    //public static void Reset2DRendererDisplay(Expression expData)
    //{
    //    for (int index = 0; index < expData.components.Count; ++index)
    //    {
    //        switch (expData.components[index].controlType - 2)
    //        {
    //            case 0:
    //                if (Object.op_Implicit((Object)expData.controllerVars[index].spriteRenderer))
    //                {
    //                    expData.controllerVars[index].spriteRenderer.sprite = (Sprite)null;
    //                    break;
    //                }
    //                break;
    //            case 1:
    //                if (Object.op_Implicit((Object)expData.controllerVars[index].uguiRenderer))
    //                {
    //                    expData.controllerVars[index].uguiRenderer.sprite = (Sprite)null;
    //                    break;
    //                }
    //                break;
    //            case 2:
    //                if (Object.op_Implicit((Object)expData.controllerVars[index].textureRenderer))
    //                {
    //                    expData.controllerVars[index].textureRenderer.sharedMaterials[expData.controllerVars[index].materialIndex].SetTexture(SalsaSettings.RenderPipelineTextureName, (Texture)null);
    //                    break;
    //                }
    //                break;
    //            case 3:
    //                if (Object.op_Implicit((Object)expData.controllerVars[index].materialRenderer))
    //                {
    //                    expData.controllerVars[index].materialRenderer.sharedMaterial = (Material)null;
    //                    break;
    //                }
    //                break;
    //        }
    //    }
    //}

    public static void DrawDoubleSlider(
      string label,
      string shortLabel,
      string tooltip,
      float inspWidth,
      ref float loCutoff,
      ref float hiCutoff)
    {
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        float num1 = 330f;
        float num2 = 50f;
        float num3 = (double)inspWidth < (double)num1 ? 87f : 125f;
        loCutoff = EditorGUILayout.FloatField(loCutoff, new GUILayoutOption[1]
        {
        GUILayout.MaxWidth(num2)
        });
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField(new GUIContent((double)inspWidth < (double)num1 ? shortLabel : label, tooltip), new GUILayoutOption[1]
        {
        GUILayout.MaxWidth(num3)
        });
        GUILayout.FlexibleSpace();
        hiCutoff = EditorGUILayout.FloatField(hiCutoff, new GUILayoutOption[1]
        {
        GUILayout.MaxWidth(num2)
        });
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.MinMaxSlider(ref loCutoff, ref hiCutoff, 0.0f, 1f, new GUILayoutOption[0]);
    }

    public static void DrawHiLoFloat(float inspWidth, ref float loCutoff, ref float hiCutoff)
    {
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        float labelWidth = EditorGUIUtility.labelWidth;
        float fieldWidth = EditorGUIUtility.fieldWidth;
        EditorGUIUtility.labelWidth = 40f;
        EditorGUIUtility.fieldWidth = 40f;
        GUILayout.FlexibleSpace();
        loCutoff = EditorGUILayout.FloatField("From", Mathf.Clamp(loCutoff, 0.0f, hiCutoff - 1f / 1000f), new GUILayoutOption[0]);
        GUILayout.Space(15f);
        hiCutoff = EditorGUILayout.FloatField("To", Mathf.Max(hiCutoff, loCutoff + 1f / 1000f), new GUILayoutOption[0]);
        EditorGUILayout.EndHorizontal();
        EditorGUIUtility.fieldWidth = fieldWidth;
        EditorGUIUtility.labelWidth = labelWidth;
    }

    public static string TruncateHeaderName(string name, float inspWidth, float truncWidth)
    {
        string str1 = name.Substring(0, Mathf.Clamp(18 + (int)(((double)inspWidth - (double)truncWidth) / 12.0), name.Length < 5 ? name.Length : 5, name.Length));
        string str2;
        return str1.Length < name.Length ? (str2 = str1 + "...") : str1;
    }

    public enum AlertType
    {
        Good,
        Normal,
        Warning,
        Error,
    }

    public enum HeaderType
    {
        Level1,
        Level2,
        Level3,
        Level4,
    }

}