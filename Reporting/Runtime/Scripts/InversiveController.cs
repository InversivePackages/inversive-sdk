using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Reporting
{
    public static class InversiveController
    {
        public static IEnumerator SendNotation(ExperienceCreateNotationModel notationModel, Texture2D screenshotImage, Action<bool> callback)
        {
            var sessionId = InversiveExperience.GetSessionId();
            if (!string.IsNullOrEmpty(sessionId))
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(notationModel));
                using (UnityWebRequest www = UnityWebRequest.Post(InversiveExperience.GetUri($"reporting/send?sessionId={sessionId}"), new WWWForm()))
                {
                    www.uploadHandler = new UploadHandlerRaw(body);
                    www.downloadHandler = new DownloadHandlerBuffer();
                    www.SetRequestHeader("Content-Type", "application/json");
                    yield return www.SendWebRequest();
                    if (www.result != UnityWebRequest.Result.Success)
                        callback(false);
                    else
                    {
                        var notationId = int.Parse(Encoding.Default.GetString(www.downloadHandler.data));
                        byte[] textureBytes = null;
                        textureBytes = screenshotImage.EncodeToPNG();
                        var form = new WWWForm();
                        form.AddBinaryData("file", textureBytes, $"uploadScreenshot.png");
                        using UnityWebRequest newWww = UnityWebRequest.Post(InversiveExperience.GetUri($"reporting/upload-screenshot?sessionId={sessionId}&id={notationId}"), form);
                        newWww.downloadHandler = new DownloadHandlerBuffer();
                        newWww.uploadHandler.contentType = "multipart/form-data";
                        yield return newWww.SendWebRequest();
                        callback(newWww.result == UnityWebRequest.Result.Success);
                    }
                }
            }
            else
            {
                Debug.LogError(InversiveUtilities.CallFailedMessage($"SendNotation Failed : Session Id not found"));
                callback(false);
            }
        }

        public static IEnumerator CheckSession(Action<bool> callback)
        {
            var sessionId = InversiveExperience.GetSessionId();
            if (!string.IsNullOrEmpty(sessionId))
            {
                using (var request = UnityWebRequest.Get(InversiveExperience.GetUri($"reporting/check?sessionId={sessionId}")))
                {
                    yield return request.SendWebRequest();
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError(InversiveUtilities.CallFailedMessage($"CheckSession Failed :  {request.error}"));
                        callback(false);
                    }
                    else
                    {
                        Debug.Log(InversiveUtilities.SuccessMessage($"CheckSession : {request.downloadHandler.text}"));
                        callback(true);
                    }
                }
            }
            else
            {
                Debug.LogError(InversiveUtilities.CallFailedMessage($"CheckSession Failed : Session Id not found"));
                callback(false);
            }
        }

    }

}
