using System.Collections.Generic;
using UnityEngine;

public class InversiveUtilities
{
    public static string TAG = "InversiveSDK";

    private const string _ApiUrl = "https://sdk.vrcxp.com/";

    public static string GetApiUrl() { return _ApiUrl; }
    public static string Message(string message) { return $"{TAG} - Message :\n {message}"; }
    public static string NotFoundMessage(string message) { return $"{TAG} - 003 Not Found :\n {message}"; }
    public static string CallFailedMessage(string message) { return $"{TAG} - 005 Calling Api Failed :\n {message}"; }
    public static string ParsingFailedMessage(string message) { return $"{TAG} - 007 Unprocessable Entity :\n {message}"; }
    public static string ValidationFailedMessage(string message) { return $"{TAG} - 011 Experience Model Validation failed :\n {message}"; }
    public static string SuccessMessage(string message) { return $"{TAG} - 002 Success :\n {message}"; }

    private static (object result, string errorMessage) TryParseActionTypeToValue(ActionTypeEnum actionType, string value)
    {
        switch (actionType)
        {
            case ActionTypeEnum.Boolean:
                if (bool.TryParse(value, out bool boolResult))
                    return (boolResult, "");
                break;
            case ActionTypeEnum.String:
                return (value, "");
            case ActionTypeEnum.Float:
                if (float.TryParse(value, out float floatResult))
                    return (floatResult, "");
                break;

            case ActionTypeEnum.Integer:
                if (int.TryParse(value, out int intResult))
                    return (intResult, "");
                break;
            default:
                return (null, $"Unsupported Type : This is not a {actionType}");
        }
        return (null, $"Unsupported Type : This is not a {actionType}");
    }

    public static bool ValidateModel(ExperienceModel model)
    {
        bool result = true;
        if (model == null)
        {
            Debug.LogError(ValidationFailedMessage("Experience model is not assigned !"));
            result = false;
            return result;
        }

        HashSet<string> actionNames = new HashSet<string>();
        HashSet<string> chapterNames = new HashSet<string>();
        HashSet<int> chapterOrders = new HashSet<int>();

        foreach (var chapter in model.Chapters)
        {
            if (!string.IsNullOrEmpty(chapter.Name))
            {
                if (chapterNames.Contains(chapter.Name))
                {
                    Debug.LogError(ValidationFailedMessage($"Duplicate chapter name found: {chapter.Name}"));
                    result = false;
                }
                else
                    chapterNames.Add(chapter.Name);
            }
            else
            {
                Debug.LogError(ValidationFailedMessage($"Chapter name cannot be null !"));
                result = false;
            }


            if (chapterOrders.Contains(chapter.Order))
            {
                Debug.LogError(ValidationFailedMessage($"Duplicate chapter order found: {chapter.Order}"));
                result = false;
            }
            else
                chapterOrders.Add(chapter.Order);

            foreach (var action in chapter.Actions)
            {
                if (!string.IsNullOrEmpty(action.Name))
                {
                    if (actionNames.Contains(action.Name))
                    {
                        Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name}, Duplicate action name found: {action.Name}"));
                        result = false;
                    }
                    else
                        actionNames.Add(action.Name);
                }
                else
                {
                    Debug.LogError(ValidationFailedMessage($"Action name cannot be null !"));
                    result = false;
                }

                switch (action.ActionResponseTypeEnum)
                {
                    case ActionResponseTypeEnum.Unique:
                        if (string.IsNullOrEmpty(action.UniqueGoodAnswer))
                        {
                            Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name}, Unique Answer must not be null!"));
                            result = false;
                        }
                        object uniqueReponse;
                        bool isUniqueGoodAnswerValid = ValidateAnswerType(chapter.Name, action, action.UniqueGoodAnswer, out uniqueReponse);
                        if (!isUniqueGoodAnswerValid)
                            result = false;
                        break;
                    case ActionResponseTypeEnum.MultipleValues:
                        if (action.MultipleValues.Count == 0)
                        {
                            Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name}, Multiple Values List must not be empty!"));
                            result = false;
                        }
                        else
                        {
                            foreach (var value in action.MultipleValues)
                            {
                                if (string.IsNullOrEmpty(value.GivenResponse))
                                {
                                    Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name}, Value {value.Id} Given Response must not be null!"));
                                    result = false;
                                }
                                object givenResponse;
                                bool isGivenResponseValid = ValidateAnswerType(chapter.Name, action, value.GivenResponse, out givenResponse);
                                if (!isGivenResponseValid)
                                    result = false;
                            }
                        }
                        break;
                    case ActionResponseTypeEnum.Interval:
                        if (string.IsNullOrEmpty(action.MaxScoreResponse))
                        {
                            Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name}, Expected Value must not be null!"));
                            result = false;
                        }
                        object expectedValue;
                        bool isMaxScoreResponseValid = ValidateAnswerType(chapter.Name, action, action.MaxScoreResponse, out expectedValue);
                        if (!isMaxScoreResponseValid)
                            result = false;
                        if (string.IsNullOrEmpty(action.MinScorePositiveResponse))
                        {
                            Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name}, Maximum Value must not be null!"));
                            result = false;
                        }
                        object maxVal;
                        bool isMinScorePositiveResponseValid = ValidateAnswerType(chapter.Name, action, action.MinScorePositiveResponse, out maxVal);
                        if (!isMinScorePositiveResponseValid)
                            result = false;
                        if (string.IsNullOrEmpty(action.MinScoreNegativeResponse))
                        {
                            Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name}, Minimum Value must not be null!"));
                            result = false;
                        }
                        object minVal;
                        bool isMinScoreNegativeResponseValid = ValidateAnswerType(chapter.Name, action, action.MinScoreNegativeResponse, out minVal);
                        if (!isMinScoreNegativeResponseValid)
                            result = false;
                        if (!IsMinUnderMax(chapter.Name, action.MinScoreNegativeResponse, action.MinScorePositiveResponse, action))
                        {
                            Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name},  Minimum value must be lower than the Maximum value"));
                            result = false;
                        }
                        break;
                    case ActionResponseTypeEnum.RatingLevel:
                        if (action.ExperienceActionRatingLevels.Count == 0)
                        {
                            Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name}, Rating Level List must not be empty!"));
                            result = false;
                        }
                        else
                        {
                            foreach (var ratingLevel in action.ExperienceActionRatingLevels)
                            {
                                if (string.IsNullOrEmpty(ratingLevel.LowerLimit))
                                {
                                    Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name}, Rating Level {ratingLevel.Id} Lower Limit must not be null!"));
                                    result = false;
                                }
                                object lowerLimit;
                                bool isLowerLimitValid = ValidateAnswerType(chapter.Name, action, ratingLevel.LowerLimit, out lowerLimit);
                                if (!isLowerLimitValid)
                                    result = false;
                                if (string.IsNullOrEmpty(ratingLevel.UpperLimit))
                                {
                                    Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name}, Rating Level {ratingLevel.Id} Upper Limit must not be null!"));
                                    result = false;
                                }
                                object upperLimit;
                                bool isUpperLimitValid = ValidateAnswerType(chapter.Name, action, ratingLevel.UpperLimit, out upperLimit);
                                if (!isUpperLimitValid)
                                    result = false;

                                if (!IsMinUnderMax(chapter.Name, ratingLevel.LowerLimit, ratingLevel.UpperLimit, action))
                                {
                                    Debug.LogError(ValidationFailedMessage($"Chapter : {chapter.Name} - Action : {action.Name},  Rating Level {ratingLevel.Id} Lower limit must be lower than the Upper limit"));
                                    result = false;
                                }

                            }
                        }
                        break;
                }
            }
        }
        return result;
    }

    private static bool ValidateAnswerType(string chapterName, ExperienceActionModel action, string value, out object result)
    {
        var parseResult = TryParseActionTypeToValue(action.ActionType, value);
        if (parseResult.result == null)
        {
            Debug.LogError(ParsingFailedMessage($"Chapter : {chapterName} - Action : {action.Name}, {parseResult.errorMessage} | Value type must be {action.ActionType}"));
            result = null;
            return false;
        }
        result = parseResult.result;
        return true;
    }

    private static bool IsMinUnderMax(string chapterName, string min, string max, ExperienceActionModel action)
    {
        bool result = true;
        switch (action.ActionType)
        {
            case ActionTypeEnum.Float:
                if (float.TryParse(min, out float minFloatResult) && float.TryParse(max, out float maxFloatResult))
                {
                    if (minFloatResult >= maxFloatResult)
                        result = false;
                }
                else
                {
                    Debug.LogError(Message($"Chapter : {chapterName} - Action : {action.Name}, Value type must be {action.ActionType}"));
                    result = false;
                }
                break;

            case ActionTypeEnum.Integer:
                if (int.TryParse(min, out int minIntResult) && int.TryParse(max, out int maxIntResult))
                {
                    if (minIntResult >= maxIntResult)
                        result = false;
                }
                else
                {
                    Debug.LogError(Message($"Chapter : {chapterName} - Action : {action.Name}, Value type must be {action.ActionType}"));
                    result = false;
                }
                break;
        }
        return result;
    }

    public static string ReturnGlobalScore(int GlobalScore, ScoringEnum scoring)
    {
        var gradeMap = new Dictionary<int, string>
        {
            { 75, "A+" },
            { 70, "A" },
            { 65, "A-" },
            { 60, "B+" },
            { 55, "B" },
            { 50, "B-" },
            { 45, "C+" },
            { 40, "C" },
            { 35, "C-" },
            { 30, "D+" },
            { 25, "D" },
            { 20, "D-" },
            { 15, "F" }
        };

        switch (scoring)
        {
            case ScoringEnum.AsIs:
                return GlobalScore.ToString();
            case ScoringEnum.Ten:
                int scoreOnTenScale = GlobalScore / 10;
                return scoreOnTenScale.ToString();
            case ScoringEnum.Twenty:
                int scoreOnTwentyScale = (int)(GlobalScore / 5.0);
                return scoreOnTwentyScale.ToString();
            case ScoringEnum.Letter:
                foreach (var grade in gradeMap)
                    if (GlobalScore >= grade.Key)
                        return grade.Value;
                return "F";
            default:
                return "Invalid ScoringEnum";
        }
    }
}
