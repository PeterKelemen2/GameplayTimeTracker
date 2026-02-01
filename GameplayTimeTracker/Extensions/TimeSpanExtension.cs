using System;

namespace GameplayTimeTracker.Extensions;

public static class TimeSpanExtension
{
    public static string GetPretty(this TimeSpan ts)
    {
        int hours = (int)ts.TotalHours;
        return $"{hours}h  {ts.Minutes:D2}m {ts.Seconds:D2}s";
    }
}