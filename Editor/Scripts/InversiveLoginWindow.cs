using System;
using System.Collections;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
public class InversiveLoginWindow : EditorWindow
{
    GUIContent InversiveIcon = null;

    private static readonly string InversiveIconPath = "Packages/com.inversive.inversive-sdk/Editor/UI/inversive-logo.png";
    private static readonly string Realm = "inversive";
    private static readonly string RealmUrl = $"https://auth.inversive.fr/realms/{Realm}";
    private static readonly string ClientId = "standalone-client";

    private GUIStyle boldLabelStyle;
    private GUIStyle centeredLabelStyle;
    private GUIStyle errorLabelStyle;
    private GUIStyle codeLabelStyle;

    private DeviceAuthorizationResponse deviceAuthResponse;
    private bool isWaitingForAuth = false;
    private string errorMessage = string.Empty;
    private EditorCoroutine pollingCoroutine;

    [MenuItem("InversiveSDK/Generate App Id", false, 1999)]
    public static void Init()
    {
        EditorWindow window = GetWindow<InversiveLoginWindow>("App Id");
        window.minSize = new Vector2(450, 400);
        window.maxSize = new Vector2(450, 400);
        window.Show();
    }

    private void OnEnable()
    {
        if (InversiveIcon == null)
        {
            InversiveIcon = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture2D>(InversiveIconPath));
        }

        if (boldLabelStyle == null)
        {
            boldLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            boldLabelStyle.fontSize = 14;
        }

        if (centeredLabelStyle == null)
        {
            centeredLabelStyle = new GUIStyle(EditorStyles.label);
            centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
            centeredLabelStyle.wordWrap = true;
        }

        if (errorLabelStyle == null)
        {
            errorLabelStyle = new GUIStyle(EditorStyles.label);
            errorLabelStyle.normal.textColor = Color.red;
            errorLabelStyle.alignment = TextAnchor.MiddleCenter;
            errorLabelStyle.wordWrap = true;
        }

        if (codeLabelStyle == null)
        {
            codeLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            codeLabelStyle.fontSize = 24;
            codeLabelStyle.alignment = TextAnchor.MiddleCenter;
            codeLabelStyle.normal.textColor = new Color(0.2f, 0.6f, 1f);
        }
    }

    private void OnDisable()
    {
        if (pollingCoroutine != null)
        {
            EditorCoroutineUtility.StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        // Logo
        using (var l = new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(InversiveIcon, GUILayout.Height(100), GUILayout.Width(200));
            GUILayout.FlexibleSpace();
        }

        GUILayout.Space(20);

        if (!isWaitingForAuth)
        {
            // Initial state - Show generate button
            GUILayout.Label("SSO Authentication", boldLabelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            GUILayout.Label("Click the button below to start the authentication process", centeredLabelStyle);
            GUILayout.Space(20);

            using (var h = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Generate Authentication Code", GUILayout.Width(220), GUILayout.Height(30)))
                {
                    StartDeviceAuthFlow();
                }
                GUILayout.FlexibleSpace();
            }
        }
        else if (deviceAuthResponse != null)
        {
            // Waiting for authentication
            GUILayout.Label("Authentication in Progress", boldLabelStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(10);

            GUILayout.Label("Please enter this code on the authentication page:", centeredLabelStyle);
            GUILayout.Space(10);

            // Display the user code prominently
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label(deviceAuthResponse.user_code, codeLabelStyle);
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(10);

            // Copy button
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Copy Code", GUILayout.Width(120)))
                {
                    EditorGUIUtility.systemCopyBuffer = deviceAuthResponse.user_code;
                    Debug.Log("Code copied to clipboard: " + deviceAuthResponse.user_code);
                }
                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(15);

            // Open browser button
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Open Authentication Page", GUILayout.Width(200), GUILayout.Height(25)))
                {
                    Application.OpenURL(deviceAuthResponse.verification_uri);
                }
                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(10);
            GUILayout.Label("Waiting for authentication...", centeredLabelStyle);

            // Cancel button
            GUILayout.Space(15);
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(100)))
                {
                    CancelAuthentication();
                }
                GUILayout.FlexibleSpace();
            }
        }

        // Error message
        if (!string.IsNullOrEmpty(errorMessage))
        {
            GUILayout.Space(20);
            GUILayout.Label(errorMessage, errorLabelStyle);
        }

        GUILayout.FlexibleSpace();
    }

    private void StartDeviceAuthFlow()
    {
        errorMessage = string.Empty;
        EditorCoroutineUtility.StartCoroutine(RequestDeviceAuthorization(), this);
    }

    private IEnumerator RequestDeviceAuthorization()
    {
        string url = $"{RealmUrl}/protocol/openid-connect/auth/device";

        WWWForm form = new WWWForm();
        form.AddField("client_id", ClientId);
        form.AddField("scope", "openid email profile");

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    deviceAuthResponse = JsonConvert.DeserializeObject<DeviceAuthorizationResponse>(request.downloadHandler.text);

                    if (deviceAuthResponse != null && !string.IsNullOrEmpty(deviceAuthResponse.device_code))
                    {
                        isWaitingForAuth = true;
                        Debug.Log("Device authorization initiated. User code: " + deviceAuthResponse.user_code);

                        // Start polling for token
                        pollingCoroutine = EditorCoroutineUtility.StartCoroutine(PollForToken(), this);
                    }
                    else
                    {
                        errorMessage = "Invalid response from authorization server.";
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "Failed to parse authorization response: " + ex.Message;
                    Debug.LogError(errorMessage);
                }
            }
            else
            {
                errorMessage = "Authentication request failed: " + request.error;
                Debug.LogError(errorMessage);
            }
        }
    }

    private IEnumerator PollForToken()
    {
        string url = $"{RealmUrl}/protocol/openid-connect/token";
        double startTime = EditorApplication.timeSinceStartup;
        int interval = deviceAuthResponse.interval > 0 ? deviceAuthResponse.interval : 5;
        int expiresIn = deviceAuthResponse.expires_in > 0 ? deviceAuthResponse.expires_in : 300;
        double nextPollTime = startTime + interval;

        while ((EditorApplication.timeSinceStartup - startTime) < expiresIn)
        {
            // Wait until it's time for the next poll
            if (EditorApplication.timeSinceStartup < nextPollTime)
            {
                yield return null;
                continue;
            }

            nextPollTime = EditorApplication.timeSinceStartup + interval;

            WWWForm form = new WWWForm();
            form.AddField("client_id", ClientId);
            form.AddField("grant_type", "urn:ietf:params:oauth:grant-type:device_code");
            form.AddField("device_code", deviceAuthResponse.device_code);

            using (UnityWebRequest request = UnityWebRequest.Post(url, form))
            {
                yield return request.SendWebRequest();

                Debug.Log($"Poll attempt - Response code: {request.responseCode}, Result: {request.result}");

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        Debug.Log("Token response received: " + request.downloadHandler.text);
                        TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(request.downloadHandler.text);

                        if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.access_token))
                        {
                            OnAuthenticationSuccess(tokenResponse.access_token);
                            yield break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Token parsing error: " + ex.Message);
                    }
                }
                else if (request.responseCode == 400)
                {
                    // Check for "authorization_pending" or "slow_down"
                    try
                    {
                        Debug.Log("Error response: " + request.downloadHandler.text);
                        var errorResponse = JsonConvert.DeserializeObject<TokenErrorResponse>(request.downloadHandler.text);

                        if (errorResponse.error == "authorization_pending")
                        {
                            Debug.Log("Authorization pending, continuing to poll...");
                            // Continue polling
                            continue;
                        }
                        else if (errorResponse.error == "slow_down")
                        {
                            Debug.Log("Slow down requested, increasing interval");
                            // Increase interval
                            interval += 5;
                            nextPollTime = EditorApplication.timeSinceStartup + interval;
                            continue;
                        }
                        else
                        {
                            errorMessage = $"Authentication error: {errorResponse.error}";
                            Debug.LogError(errorMessage);
                            CancelAuthentication();
                            yield break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Error parsing error response: " + ex.Message);
                        errorMessage = "Authentication failed.";
                        CancelAuthentication();
                        yield break;
                    }
                }
                else
                {
                    Debug.LogError($"Unexpected response: {request.responseCode} - {request.error}");
                }
            }
        }

        errorMessage = "Authentication timeout. Please try again.";
        CancelAuthentication();
    }
    private void OnAuthenticationSuccess(string keycloakAccessToken)
    {
        Debug.Log("Keycloak authentication successful! Generating experience access token...");
        EditorCoroutineUtility.StartCoroutine(GenerateExperienceAccessToken(keycloakAccessToken), this);
    }
    private IEnumerator GenerateExperienceAccessToken(string keycloakAccessToken)
    {
        string url = InversiveUtilities.ApiUrl + "account/generate-access-token";

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            // Add the Keycloak access token as Bearer token in the Authorization header
            request.SetRequestHeader("Authorization", "Bearer " + keycloakAccessToken);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    // Parse the response to get the experience access token
                    string experienceAccessToken = request.downloadHandler.text.Trim('"'); // Remove quotes if it's a plain string

                    if (!string.IsNullOrEmpty(experienceAccessToken))
                    {
                        // Set the experience access token
                        InversiveService.SetAccessToken(experienceAccessToken);
                        Debug.Log("Experience access token generated successfully!");

                        EditorUtility.DisplayDialog(
                            "Authentication Successful!",
                            "Your project is now linked to your Inversive account.\n\n",
                            "Ok"
                        );

                        Close();
                        ExperienceEditor.ShowWindowAfterLogin();
                    }
                    else
                    {
                        errorMessage = "Failed to generate experience access token: Empty response";
                        Debug.LogError(errorMessage);
                        CancelAuthentication();
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "Failed to parse experience access token: " + ex.Message;
                    Debug.LogError(errorMessage);
                    CancelAuthentication();
                }
            }
            else
            {
                errorMessage = $"Failed to generate experience access token: {request.error} (Code: {request.responseCode})";
                Debug.LogError(errorMessage + " - Response: " + request.downloadHandler.text);
                CancelAuthentication();
            }
        }
    }

    private void CancelAuthentication()
    {
        if (pollingCoroutine != null)
        {
            EditorCoroutineUtility.StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }

        isWaitingForAuth = false;
        deviceAuthResponse = null;
    }

    [Serializable]
    private class DeviceAuthorizationResponse
    {
        public string device_code;
        public string user_code;
        public string verification_uri;
        public string verification_uri_complete;
        public int expires_in;
        public int interval;
    }

    [Serializable]
    private class TokenResponse
    {
        public string access_token;
        public string token_type;
        public int expires_in;
        public string refresh_token;
        public string scope;
    }

    [Serializable]
    private class TokenErrorResponse
    {
        public string error;
        public string error_description;
    }
}