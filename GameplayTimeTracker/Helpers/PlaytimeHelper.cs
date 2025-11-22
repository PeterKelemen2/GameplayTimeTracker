using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Helpers;

public static class PlaytimeHelper
{
    public static void NormalizeTime(Playtime playtime)
    {
        if (playtime.Seconds >= 60)
        {
            playtime.Minutes += playtime.Seconds / 60;
            playtime.Seconds %= 60;
        }

        if (playtime.Minutes >= 60)
        {
            playtime.Hours += playtime.Minutes / 60;
            playtime.Minutes %= 60;
        }
    }
}