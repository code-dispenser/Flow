namespace Flow.Core.Tests.SharedDataAndFixtures.Common.Utilities;

public static class FailureUtility
{
    public static object CreateFailure(Type failureType, string? reason, Dictionary<string, string>? details, int subTypeID, bool canRetry, Exception? exception, DateTime? occurredAt)
    {
        Type stringType = typeof(string);
        Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(stringType, stringType);

        return failureType.GetConstructor([stringType, dictionaryType, typeof(int), typeof(bool), typeof(Exception), typeof(DateTime)])!
                        .Invoke([reason, details, subTypeID, canRetry, exception, occurredAt]);

    }

    public static T CreateFailure<T>(string? reason, Dictionary<string, string>? details, int subTypeID, bool canRetry, Exception? exception, DateTime? occurredAt)
    {
        Type stringType = typeof(string);
        Type dictionaryType = typeof(Dictionary<,>).MakeGenericType(stringType, stringType);

        return (T) typeof(T).GetConstructor([stringType, dictionaryType, typeof(int), typeof(bool), typeof(Exception), typeof(DateTime)])!
                                .Invoke([reason, details, subTypeID, canRetry, exception, occurredAt]);

    }
}
