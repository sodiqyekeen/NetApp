namespace NetApp.UI.Infrastructure.Extensions;

public static class DateTimeExtensions
{
    public static string GetTimeDifference(this DateTime dateTime)
    {
        TimeSpan timeDifference = DateTime.Now - dateTime.ToLocalTime();

        return timeDifference switch
        {
            { TotalDays: >= 2 } => $"{timeDifference.TotalDays} days ago",
            { TotalDays: 1 } => "Yesterday",
            { TotalHours: >= 1 } => $"{timeDifference.TotalHours:0} hours ago",
            _ => $"{timeDifference.TotalMinutes:0} mins ago",
        };
    }
}
