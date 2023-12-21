using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InversiveUtilities
{
    public static string TAG = "InversiveSDK";

    private const string _ApiUrl = "https://sdk.vrcxp.com/";

    public static string GetApiUrl() { return _ApiUrl; }

    public static string Message(string message) { return $"{TAG} : {message}"; }

    private static ExperienceModel ExperienceModel
    {
        get { return InversiveService.GetExperience(); }
    }

    public static int? CalculateScore(ExperienceChapterModel chapter, ExperienceActionModel action, List<string> value)
    {
        int? score = null;
        var parseResult = TryParseActionTypeToValue(action.ActionType, value);
        if (parseResult.result == null)
        {
            Debug.LogError(Message(parseResult.errorMessage));
        }

        var actionValues = parseResult.result;

        switch (action.ActionResponseTypeEnum)
        {
            case ActionResponseTypeEnum.Unique:
                var unique = action;
                if (unique != null)
                {
                    if (actionValues is List<bool> boolValue)
                        score = boolValue.First() == bool.Parse(unique.UniqueGoodAnswer) ? 100 : unique.WrongUniqueAnswerScore;
                    else if (actionValues is List<float> floatValue)
                        score = floatValue.First() == float.Parse(unique.UniqueGoodAnswer) ? 100 : unique.WrongUniqueAnswerScore;
                    else if (actionValues is List<int> intValue)
                        score = intValue.First() == int.Parse(unique.UniqueGoodAnswer) ? 100 : unique.WrongUniqueAnswerScore;
                    else if (actionValues is List<string> stringValue)
                        score = stringValue.First() == unique.UniqueGoodAnswer ? 100 : unique.WrongUniqueAnswerScore;
                }
                break;
            case ActionResponseTypeEnum.Interval:
                var interval = action;
                if (interval != null)
                {
                    if (actionValues is List<float> floatValue)
                    {
                        if (float.TryParse(interval.MinScoreNegativeResponse, out var borneMin) &&
                        float.TryParse(interval.MinScorePositiveResponse, out var borneSup) &&
                        float.TryParse(interval.MaxScoreResponse, out var centerValue))
                            score = CalculateIntervalNote(borneSup, borneMin, centerValue, floatValue.First());

                    }
                    else if (actionValues is List<int> intValue)
                    {
                        if (int.TryParse(interval.MinScoreNegativeResponse, out var borneMin) &&
                        int.TryParse(interval.MinScorePositiveResponse, out var borneSup) &&
                        int.TryParse(interval.MaxScoreResponse, out var centerValue))
                            score = CalculateIntervalNote(borneSup, borneMin, centerValue, intValue.First());
                    }
                }
                break;
            case ActionResponseTypeEnum.MultipleValues:
                var values = action.MultipleValues;
                if (values != null)
                {
                    if (actionValues is List<bool> boolValue)
                        score = boolValue.SelectMany(b => values.Where(x =>
                        {
                            if (bool.TryParse(x.GivenResponse, out var e))
                                return e == b;
                            return false;
                        }).Select(x => x.Score)).Sum();
                    else if (actionValues is List<float> floatValue)
                        score = floatValue.SelectMany(b => values.Where(x =>
                        {
                            if (float.TryParse(x.GivenResponse, out var e))
                                return e == b;
                            return false;
                        }).Select(x => x.Score)).Sum();
                    else if (actionValues is List<int> intValue)
                        score = intValue.SelectMany(b => values.Where(x =>
                        {
                            if (int.TryParse(x.GivenResponse, out var e))
                                return e == b;
                            return false;
                        }).Select(x => x.Score)).Sum();
                    else if (actionValues is List<string> stringValue)
                        score = stringValue.SelectMany(b => values.Where(x => x.GivenResponse == b).Select(x => x.Score)).Sum();
                }
                break;
            case ActionResponseTypeEnum.RatingLevel:
                var ratingLevels = action.ExperienceActionRatingLevels;
                if (ratingLevels != null)
                {
                    if (actionValues is List<float> floatValue)
                    {
                        score = ratingLevels.Any(x => float.TryParse(x.LowerLimit, out var lowerLimit) && float.TryParse(x.UpperLimit, out var upperLimit)
                        && floatValue.First() >= lowerLimit && floatValue.First() <= upperLimit)
                        ? ratingLevels.First(x => float.TryParse(x.LowerLimit, out var lowerLimit) && float.TryParse(x.UpperLimit, out var upperLimit)
                        && floatValue.First() >= lowerLimit && floatValue.First() <= upperLimit).GivenScore : 0;
                    }
                    else if (actionValues is List<int> intValue)
                    {
                        score = ratingLevels.Any(x => int.TryParse(x.LowerLimit, out var lowerLimit) && int.TryParse(x.UpperLimit, out var upperLimit)
                        && intValue.First() >= lowerLimit && intValue.First() <= upperLimit)
                        ? ratingLevels.First(x => int.TryParse(x.LowerLimit, out var lowerLimit) && int.TryParse(x.UpperLimit, out var upperLimit)
                        && intValue.First() >= lowerLimit && intValue.First() <= upperLimit).GivenScore : 0;
                    }
                }
                break;
        }

        return score;
    }

    public static int CalculateIntervalNote(object ymax, object ymin, object y, object x)
    {
        double ymaxVal = Convert.ToDouble(ymax);
        double yminVal = Convert.ToDouble(ymin);
        double yVal = Convert.ToDouble(y);
        double xVal = Convert.ToDouble(x);

        if (xVal == yVal)
        {
            return 100;
        }
        else if (xVal < yminVal || xVal > ymaxVal)
        {
            return 0;
        }
        else
        {
            double distance = Math.Abs(xVal - yVal);
            double intervalSize = Math.Abs(ymaxVal - yminVal);
            double score = (1 - (distance / intervalSize)) * 100;
            return (int)score;
        }
    }

    public static (object result, string errorMessage) TryParseActionTypeToValue(ActionTypeEnum actionType, List<string> values)
    {
        switch (actionType)
        {
            case ActionTypeEnum.Boolean:
                List<bool> boolResults = new List<bool>();
                foreach (string value in values)
                {
                    if (bool.TryParse(value, out bool boolResult))
                    {
                        boolResults.Add(boolResult);
                    }
                    else
                    {
                        return (null, $"Failed to parse value '{value}' to boolean");
                    }
                }
                return (boolResults, "");

            case ActionTypeEnum.String:
                return (values, "");

            case ActionTypeEnum.Float:
                List<float> floatResults = new List<float>();
                foreach (string value in values)
                {
                    if (float.TryParse(value, out float floatResult))
                    {
                        floatResults.Add(floatResult);
                    }
                    else
                    {
                        return (null, $"Failed to parse value '{value}' to float");
                    }
                }
                return (floatResults, "");

            case ActionTypeEnum.Integer:
                List<int> intResults = new List<int>();
                foreach (string value in values)
                {
                    if (int.TryParse(value, out int intResult))
                    {
                        intResults.Add(intResult);
                    }
                    else
                    {
                        return (null, $"Failed to parse value '{value}' to integer");
                    }
                }
                return (intResults, "");

            default:
                return (null, $"Unsupported Type : This is not a {actionType}");
        }
    }

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
            Debug.LogError(Message("Experience model is not assigned!"));
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
                    Debug.LogError(Message($"Duplicate chapter name found: {chapter.Name}"));
                    result = false;
                }
                else
                    chapterNames.Add(chapter.Name);
            }
            else
            {
                Debug.LogError(Message($"Chapter name cannot be null !"));
                result = false;
            }


            if (chapterOrders.Contains(chapter.Order))
            {
                Debug.LogError(Message($"Duplicate chapter order found: {chapter.Order}"));
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
                        Debug.LogError(Message($"Chapter : {chapter.Name}, Duplicate action name found: {action.Name}"));
                        result = false;
                    }
                    else
                        actionNames.Add(action.Name);
                }
                else
                {
                    Debug.LogError(Message($"Action name cannot be null !"));
                    result = false;
                }

                switch (action.ActionResponseTypeEnum)
                {
                    case ActionResponseTypeEnum.Unique:
                        if (string.IsNullOrEmpty(action.UniqueGoodAnswer))
                        {
                            Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name}, Unique Answer must not be null!"));
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
                            Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name}, Multiple Values List must not be empty!"));
                            result = false;
                        }
                        else
                        {
                            foreach (var value in action.MultipleValues)
                            {
                                if (string.IsNullOrEmpty(value.GivenResponse))
                                {
                                    Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name}, Value {value.Id} Given Response must not be null!"));
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
                            Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name}, Expected Value must not be null!"));
                            result = false;
                        }
                        object expectedValue;
                        bool isMaxScoreResponseValid = ValidateAnswerType(chapter.Name, action, action.MaxScoreResponse, out expectedValue);
                        if (!isMaxScoreResponseValid)
                            result = false;
                        if (string.IsNullOrEmpty(action.MinScorePositiveResponse))
                        {
                            Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name}, Maximum Value must not be null!"));
                            result = false;
                        }
                        object maxVal;
                        bool isMinScorePositiveResponseValid = ValidateAnswerType(chapter.Name, action, action.MinScorePositiveResponse, out maxVal);
                        if (!isMinScorePositiveResponseValid)
                            result = false;
                        if (string.IsNullOrEmpty(action.MinScoreNegativeResponse))
                        {
                            Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name}, Minimum Value must not be null!"));
                            result = false;
                        }
                        object minVal;
                        bool isMinScoreNegativeResponseValid = ValidateAnswerType(chapter.Name, action, action.MinScoreNegativeResponse, out minVal);
                        if (!isMinScoreNegativeResponseValid)
                            result = false;
                        if (!IsMinUnderMax(chapter.Name, action.MinScoreNegativeResponse, action.MinScorePositiveResponse, action))
                        {
                            Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name},  Minimum value must be lower than the Maximum value"));
                            result = false;
                        }
                        break;
                    case ActionResponseTypeEnum.RatingLevel:
                        if (action.ExperienceActionRatingLevels.Count == 0)
                        {
                            Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name}, Rating Level List must not be empty!"));
                            result = false;
                        }
                        else
                        {
                            foreach (var ratingLevel in action.ExperienceActionRatingLevels)
                            {
                                if (string.IsNullOrEmpty(ratingLevel.LowerLimit))
                                {
                                    Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name}, Rating Level {ratingLevel.Id} Lower Limit must not be null!"));
                                    result = false;
                                }
                                object lowerLimit;
                                bool isLowerLimitValid = ValidateAnswerType(chapter.Name, action, ratingLevel.LowerLimit, out lowerLimit);
                                if (!isLowerLimitValid)
                                    result = false;
                                if (string.IsNullOrEmpty(ratingLevel.UpperLimit))
                                {
                                    Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name}, Rating Level {ratingLevel.Id} Upper Limit must not be null!"));
                                    result = false;
                                }
                                object upperLimit;
                                bool isUpperLimitValid = ValidateAnswerType(chapter.Name, action, ratingLevel.UpperLimit, out upperLimit);
                                if (!isUpperLimitValid)
                                    result = false;

                                if (!IsMinUnderMax(chapter.Name, ratingLevel.LowerLimit, ratingLevel.UpperLimit, action))
                                {
                                    Debug.LogError(Message($"Chapter : {chapter.Name} - Action : {action.Name},  Rating Level {ratingLevel.Id} Lower limit must be lower than the Upper limit"));
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
            Debug.LogError(Message($"Chapter : {chapterName} - Action : {action.Name}, {parseResult.errorMessage} | Value type must be {action.ActionType}"));
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
            { 90, "A" },
            { 80, "B" },
            { 70, "C" },
            { 60, "D" },
            { 50, "E" },
            { 0, "F" }
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
