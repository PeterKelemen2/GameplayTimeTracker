using System;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Helpers;

public static class PlaytimeHelper
{
    // public static void NormalizeTime(Playtime playtime)
    // {
    //     if (playtime.Seconds >= 60)
    //     {
    //         playtime.Minutes += playtime.Seconds / 60;
    //         playtime.Seconds %= 60;
    //     }
    //
    //     if (playtime.Minutes >= 60)
    //     {
    //         playtime.Hours += playtime.Minutes / 60;
    //         playtime.Minutes %= 60;
    //     }
    // }

    public static (int, int, int) NormalizeTime(TimeSpan duration)
    {
        int[] arr = new[] { duration.Hours, duration.Minutes, duration.Seconds };

        for (int i = arr.Length - 1; i >= 1; i--)
        {
            if (arr[i] >= 60)
            {
                arr[i - 1] += arr[i] / 60;
                arr[i] %= 60;
            }
        }

        return (arr[0], arr[1], arr[2]);
    }

    public static (int, int, int) GetDurationFromDates(DateTime start, DateTime end)
    {
        var duration = end - start;
        var hours = duration.Hours;
        var minutes = duration.Minutes;
        var seconds = duration.Seconds;
        return (hours, minutes, seconds);
    }
}