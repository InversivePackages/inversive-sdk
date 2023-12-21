using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public partial class ExperienceSessionModel
{
    [field: SerializeField] public string Id { get; set; }
    [field: SerializeField] public string Json { get; set; }
    [field: SerializeField] public int GlobalScore { get; set; }
    [field: SerializeField] public DateTime? StartDate { get; set; }
    [field: SerializeField] public DateTime? LastUpdateDate { get; set; }
    [field: SerializeField] public DateTime? EndDate { get; set; }
    [field: SerializeField] public TimeSpan? TimePlayed { get; set; }
    [field: SerializeField] public int? CurrentChapterId { get; set; }
    [field: SerializeField] public int? LastActionId { get; set; }
    [field: SerializeField] public ExperienceModel Experience { get; set; }
    [field: SerializeField] public ExperienceChapterModel CurrentChapter { get; set; }
    [field: SerializeField] public ExperienceActionModel LastAction { get; set; }
    [field: SerializeField] public List<ExperienceSessionActionModel> ExperienceSessionActions { get; set; }
}

[Serializable]
public class ExperienceModel
{
    [field: SerializeField] public int WinScore { get; set; }
    [field: SerializeField] public ScoringEnum ScoringType { get; set; }
    public ExperienceModel()
    {
        Chapters = new List<ExperienceChapterModel>();
    }
    [field: SerializeField] public List<ExperienceChapterModel> Chapters { get; set; }
}

[Serializable]
public class ExperienceChapterModel
{
    public ExperienceChapterModel()
    {
        Actions = new List<ExperienceActionModel>();
    }

    [JsonIgnore] public bool isFoldout { get; set; }
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public int Order { get; set; }
    [field: SerializeField] public List<ExperienceActionModel> Actions { get; set; }
}

[Serializable]
public class ExperienceActionModel
{
    public ExperienceActionModel()
    {
        ExperienceActionRatingLevels = new List<ExperienceActionRatingLevelModel>();
        MultipleValues = new List<ExperienceActionValueModel>();
    }

    [JsonIgnore] public bool isFoldout { get; set; }
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public int ExperienceChapterId { get; set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public ActionTypeEnum ActionType { get; set; }
    [field: SerializeField] public string UniqueGoodAnswer { get; set; }
    [field: SerializeField] public int WrongUniqueAnswerScore { get; set; }
    [field: SerializeField] public string MinScorePositiveResponse { get; set; }
    [field: SerializeField] public string MinScoreNegativeResponse { get; set; }
    [field: SerializeField] public string MaxScoreResponse { get; set; }
    [field: SerializeField] public ActionResponseTypeEnum ActionResponseTypeEnum { get; set; }
    [field: SerializeField] public bool IsMandatory { get; set; }
    [field: SerializeField] public int? PreviousActionId { get; set; }
    [field: SerializeField] public bool HasPreviousAction { get; set; }
    [field: SerializeField] public ExperienceActionModel PreviousAction { get; set; }
    [field: SerializeField] public List<ExperienceActionRatingLevelModel> ExperienceActionRatingLevels { get; set; }
    [field: SerializeField] public List<ExperienceActionValueModel> MultipleValues { get; set; }
}

[Serializable]
public class ExperienceActionRatingLevelModel
{

    [JsonIgnore] public bool isFoldout { get; set; }
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public int GivenScore { get; set; }
    [field: SerializeField] public string UpperLimit { get; set; }
    [field: SerializeField] public string LowerLimit { get; set; }
    [field: SerializeField] public ExperienceActionInfoModel ExperienceAction { get; set; }
}

[Serializable]
public class ExperienceActionValueModel
{
    [JsonIgnore] public bool isFoldout { get; set; }
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public int Score { get; set; }
    [field: SerializeField] public string GivenResponse { get; set; }
    [field: SerializeField] public ExperienceActionInfoModel ExperienceAction { get; set; }
}

[Serializable]
public class ExperienceActionInfoModel
{
    [field: SerializeField] public int Id { get; set; }
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public bool IsToggleAction { get; set; }
    [field: SerializeField] public ActionTypeEnum ActionType { get; set; }
    [field: SerializeField] public ActionResponseTypeEnum ActionResponseTypeEnum { get; set; }
    [field: SerializeField] public string UniqueGoodAnswer { get; set; }
    [field: SerializeField] public string MaxScoreResponse { get; set; }
    [field: SerializeField] public string MinScoreNegativeResponse { get; set; }
    [field: SerializeField] public string MinScorePositiveResponse { get; set; }
    [field: SerializeField] public bool IsMandatory { get; set; }
    [field: SerializeField] public bool HasPreviousAction { get; set; }
}

[Serializable]
public class ExperienceSessionActionModel
{
    [field: SerializeField] public ExperienceActionModel RelatedAction { get; set; }
    [field: SerializeField] public List<string> Values { get; set; }
    [field: SerializeField] public int Score { get; set; }
}

[Serializable]
public class ExperienceSessionSummaryModel
{
    public string DisplayedGlobalScore { get; set; }
    public int GlobalScore { get; set; }
    public int WinScore { get; set; }
    public bool HasSucceeded { get; set; }
    public ScoringEnum ScoringType { get; set; }
    public List<ExperienceChapterSummaryModel> Chapters { get; set; }
}

[Serializable]
public class ExperienceActionSummaryModel
{
    public string Name { get; set; }
    public string Value { get; set; }
    public int Score { get; set; }
    public bool IsMandatory { get; set; }
    public int? PreviousActionId { get; set; }
}

[Serializable]
public class ExperienceChapterSummaryModel
{
    public string Name { get; set; }
    public int Order { get; set; }
    public ExperienceActionSummaryModel Actions { get; set; }
}

[Serializable]
public class LoginModel
{
    [field: SerializeField] public string ProductName { get; set; }
    [field: SerializeField] public string Email { get; set; }
    [field: SerializeField] public string Password { get; set; }
}

[Serializable]
public enum ActionTypeEnum
{
    Boolean = 0,
    String = 1,
    Float = 2,
    Integer = 3
}

[Serializable]
public class SDKError
{
    [JsonProperty("InversiveSDKError")]
    public string[] InversiveSDKError { get; set; }
}

[Serializable]
public enum ActionResponseTypeEnum
{
    Unique = 0,
    MultipleValues = 1,
    Interval = 2,
    RatingLevel = 3
}

[Serializable]
public enum ScoringEnum
{
    AsIs = 0,
    Ten = 1,
    Twenty = 2,
    Letter = 3
}
