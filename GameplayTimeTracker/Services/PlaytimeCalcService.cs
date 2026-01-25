using System;
using GameplayTimeTracker.Dtos;

namespace GameplayTimeTracker.Services;

public static class PlaytimeCalcService
{
    public static DurationDto NormalizeTime(TimeSpan duration)
    {
        int[] arr = [(int)duration.TotalHours, duration.Minutes, duration.Seconds];

        for (int i = arr.Length - 1; i >= 1; i--)
        {
            if (arr[i] >= 60)
            {
                arr[i - 1] += arr[i] / 60;
                arr[i] %= 60;
            }
        }

        return new DurationDto(arr[0], arr[1], arr[2]);
    }

    public static DurationDto GetDurationFromDates(DateTime start, DateTime end)
    {
        var duration = NormalizeTime(end - start);
        var hours = duration.Hours;
        var minutes = duration.Minutes;
        var seconds = duration.Seconds;
        return new DurationDto(hours, minutes, seconds);
    }

    public static String GetDurationFromDatesToString(DateTime start, DateTime end)
    {
        var dur = GetDurationFromDates(start, end);
        return $"{dur.Hours}h {dur.Minutes}m {dur.Seconds}s";
    }
}