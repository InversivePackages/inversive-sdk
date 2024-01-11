using Newtonsoft.Json;
using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class InversiveLoginWindow : EditorWindow
{
    LoginModel LoginModel = new();
    GUIContent InversiveIcon = null;

    private static readonly string InversiveIconPath = "Packages/com.inversive.inversive-sdk/Editor/UI/inversive-logo.png";
    private bool LoginFailed = false;
    private GUIStyle redLabelStyle;

    [MenuItem("InversiveSDK/Generate App Id", false, 1999)]
    public static void Init()
    {
        EditorWindow window = GetWindow<InversiveLoginWindow>("App Id");
        window.minSize = new Vector2(450, 300);
        window.maxSize = new Vector2(450, 300);
        window.Show();
    }

    private void OnEnable()
    {
        if (InversiveIcon == null)
        {
            InversiveIcon = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(InversiveIconPath));
        }

        if (redLabelStyle == null)
        {
            redLabelStyle = new GUIStyle(EditorStyles.label);
            redLabelStyle.normal.textColor = Color.red;
        }
    }

    private void OnGUI()
    {
        // Center Vertically
        GUILayout.FlexibleSpace();

        // Horizontally centered
        using (var l = new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(InversiveIcon, GUILayout.Height(100), GUILayout.Width(200));
            GUILayout.FlexibleSpace();
        }

        GUILayout.FlexibleSpace();

        GUILayout.Label("Email:");
        LoginModel.Email = EditorGUILayout.TextField(LoginModel.Email?.Trim());
        GUILayout.Label("Password:");
        LoginModel.Password = EditorGUILayout.PasswordField(LoginModel.Password);
        GUILayout.Space(20);

        if (GUILayout.Button("Login to generate app id"))
        {
            // Perform login validation here
            if (!string.IsNullOrEmpty(LoginModel.Email) && !string.IsNullOrEmpty(LoginModel.Password))
            {
                LoginFailed = false;
                DevLogin(LoginModel);
            }
            else
            {
                LoginFailed = true;
                Debug.Log("Login failed. Invalid credentials.");
            }
        }

        if (LoginFailed)
        {
            GUILayout.Space(16);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent("Failed to log in"), redLabelStyle);
            GUILayout.Space(64);
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// Login for developers on unity.
    /// </summary>
    /// <param name="model">Login model : 
    /// * Email : string
    /// * Password : string
    /// </param>
    private void DevLogin(LoginModel model)
    {
        string sessionId = string.Empty;
        EditorCoroutineUtility.StartCoroutine(Login(model, (x) =>
        {
            if (x.result != UnityWebRequest.Result.Success)
            {
                LoginFailed = true;
                Debug.Log("Login failed!");
            }
            else
            {
                LoginFailed = false;
                sessionId = x.downloadHandler.text;
                if (!string.IsNullOrEmpty(sessionId))
                    InversiveService.SetAccessToken(sessionId);
                EditorUtility.DisplayDialog("Token Created !", "You can now access to the experience editor\n\n", "Ok");
                Close();
                ExperienceEditor.ShowWindowAfterLogin();
            }
        }), this);
    }

    private static IEnumerator Login(LoginModel model, Action<UnityWebRequest> request = null)
    {
        if (model != null)
        {
            var json = JsonConvert.SerializeObject(model);
            using (var req = UnityWebRequest.Put(GetUri($"Account/login"), json))
            {
                req.method = "POST";
                req.SetRequestHeader("Content-Type", "application/json");
                yield return req.SendWebRequest();
                request(req);
            }
        }
        yield return null;
    }

    private static Uri GetUri(string relativePath)
    {
        var baseUri = new Uri(InversiveUtilities.GetApiUrl());
        return new Uri(baseUri, relativePath);
    }

}
