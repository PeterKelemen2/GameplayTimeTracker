using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using craftersmine.SteamGridDBNet;

namespace GameplayTimeTracker.SGDB;

public static class SGDBFetch
{
    public static async Task FetchSGDBAsync(string apiKey, string gameName)
    {
        SteamGridDb sgdb = new SteamGridDb(apiKey);
        SteamGridDbGame[]? games = await sgdb.SearchForGamesAsync(gameName);
        var game = games?.FirstOrDefault();

        if (game != null)
        {
            var heroes = await sgdb.GetHeroesByGameIdAsync(game.Id);
            var hero = heroes?.FirstOrDefault();
            if (hero != null)
            {
                Console.WriteLine(hero.FullImageUrl);
                await SGDBDownloader.DownloadImageAsync(hero.FullImageUrl,
                    Path.Combine(AppFiles.SGDBFolder, $"{game.Id}_hero.png"),
                    sizeLimits: new[] { 960, 310 });
            }

            var icons = await sgdb.GetIconsByGameIdAsync(game.Id);
            var icon = icons?.FirstOrDefault();
            if (icon != null)
            {
                if (icon.Format == SteamGridDbFormats.Ico)
                {
                    await SGDBDownloader.DownloadAndProcessIcoAsync(icon.FullImageUrl,
                        Path.Combine(AppFiles.SGDBFolder, $"{game.Id}_icon.png"));
                }
                else
                {
                    await SGDBDownloader.DownloadImageAsync(icon.FullImageUrl,
                        Path.Combine(AppFiles.SGDBFolder, $"{game.Id}_icon.png"),
                        sizeLimits: new[] { 256, 256 });
                }
            }
        }
    }
}