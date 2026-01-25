using System;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Extensions;

public static class PlaytimeExtension
{
    extension(Playtime playtime)
    {
        public TimeSpan GetDuration() => playtime.EndDate - playtime.StartDate;
    }
}