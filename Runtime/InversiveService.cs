using Newtonsoft.Json;
using UnityEngine;

public class InversiveService
{
                                                       
    private static string AccessToken;

    public static void SetAccessToken(string sessionId)
    {
        PlayerPrefs.SetString("AccessToken", sessionId);
        AccessToken = sessionId;
    }

    public static string GetAccessToken()
    {
        if (string.IsNullOrEmpty(AccessToken))
            AccessToken = PlayerPrefs.GetString("AccessToken", "");
        return AccessToken;
    }

    private static ExperienceModel ExperienceLocal;

    public static void SetExperience(ExperienceModel experience)
    {
        var json = JsonConvert.SerializeObject(experience);
        PlayerPrefs.SetString("ExperienceLocal", json);
        ExperienceLocal = experience;
    }

    public static ExperienceModel GetExperience()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("ExperienceLocal")))
            ExperienceLocal = JsonConvert.DeserializeObject<ExperienceModel>(PlayerPrefs.GetString("ExperienceLocal"));
        if (ExperienceLocal == null)
            ExperienceLocal = new ExperienceModel();
        return ExperienceLocal;
    }

    private static ExperienceModel ExperienceHead;

    public static void SetExperienceHead(ExperienceModel experience)
    {
        var json = JsonConvert.SerializeObject(experience);
        PlayerPrefs.SetString("ExperienceHead", json);
        ExperienceHead = experience;
    }

    public static ExperienceModel GetExperienceHead()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("ExperienceHead")))
            ExperienceHead = JsonConvert.DeserializeObject<ExperienceModel>(PlayerPrefs.GetString("ExperienceHead"));
        return ExperienceHead;
    }

    private static ExperienceSessionModel ExperienceSession;

    public static void SetExperienceSession(ExperienceSessionModel session)
    {
        var json = JsonConvert.SerializeObject(session);
        PlayerPrefs.SetString($"ExperienceSession", json);
        ExperienceSession = session;
    }

    public static ExperienceSessionModel GetExperienceSession()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString($"ExperienceSession")))
            ExperienceSession = JsonConvert.DeserializeObject<ExperienceSessionModel>(PlayerPrefs.GetString("ExperienceSession"));
        if (ExperienceSession == null)
            ExperienceSession = new ExperienceSessionModel();
        return ExperienceSession;
    }
}
