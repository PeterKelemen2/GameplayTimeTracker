using System;
using GameplayTimeTracker.Dtos;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Helpers;

public static class PlaytimeHelper
{
    public static DurationDto NormalizeTime(TimeSpan duration)
    {
        int[] arr = [duration.Hours, duration.Minutes, duration.Seconds];

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
        var duration = end - start;
        var hours = duration.Hours;
        var minutes = duration.Minutes;
        var seconds = duration.Seconds;
        return new DurationDto(hours, minutes, seconds);
    }
}