using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class InversiveService
{
    private static readonly string DirectoryPath = Path.Combine(Application.dataPath, "Inversive SDK");
    private static readonly string FilePath = Path.Combine(DirectoryPath, "AccessToken.txt");

    private static string AccessToken;

    public static void SetAccessToken(string appId)
    {
        try
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            File.WriteAllText(FilePath, appId);
            AccessToken = appId;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to write access token to file: {ex.Message}");
        }
    }

    public static string GetAccessToken()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                AccessToken = File.ReadAllText(FilePath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to read access token from file: {ex.Message}");
        }
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
