using System.Diagnostics;

namespace NHSv2.Communications.Application.Helpers;

public static class ActivityExtensions
{
    public static void LogExceptionEvent(
        this Activity activity,
        string exceptionType,
        Exception exception,
        string eventName,
        IList<KeyValuePair<string, object?>>? additionalTags = null)
    {
        var tags = new List<KeyValuePair<string, object?>>
        {
            new(exceptionType, exception)
        };

        if (additionalTags is { Count: > 0 })
        {
            tags.AddRange(additionalTags);
        }

        var tagsCollection = new ActivityTagsCollection(tags);
        activity.AddEvent(new ActivityEvent(eventName, default, tagsCollection));
        activity.SetStatus(ActivityStatusCode.Error);
    }
}