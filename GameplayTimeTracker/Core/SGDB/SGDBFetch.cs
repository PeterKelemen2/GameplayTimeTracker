using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using craftersmine.SteamGridDBNet;

namespace GameplayTimeTracker.SGDB;

public class SGDBFetchResult
{
    public bool GameFound { get; set; } = false;
    public bool IconFound { get; set; } = false;
    public bool HeroFound { get; set; } = false;
}

public static class SGDBFetch
{
    public static async Task<SGDBFetchResult> FetchSGDBAsync(string apiKey, string gameName,
        Dictionary<string, string> files)
    {
        if (!Path.Exists(AppFiles.SavedImagesPath))
        {
            Directory.CreateDirectory(AppFiles.SavedImagesPath);
        }

        SteamGridDb sgdb = new SteamGridDb(apiKey);
        SteamGridDbGame[]? games = await sgdb.SearchForGamesAsync(gameName);
        var game = games?.FirstOrDefault();

        SGDBFetchResult result = new SGDBFetchResult { GameFound = game != null };

        if (game != null)
        {
            var heroes = await sgdb.GetHeroesByGameIdAsync(game.Id);
            var hero = heroes?.FirstOrDefault();
            if (hero != null)
            {
                Console.WriteLine(hero.FullImageUrl);
                result.HeroFound = await SGDBDownloader.DownloadImageAsync(hero.FullImageUrl,
                    Path.Combine(AppFiles.SavedImagesPath, files["hero"]),
                    sizeLimits: new[] { 960, 310 });
            }

            var icons = await sgdb.GetIconsByGameIdAsync(game.Id);
            var icon = icons?.FirstOrDefault();
            if (icon != null)
            {
                bool iconResult;
                if (icon.Format == SteamGridDbFormats.Ico)
                {
                    result.IconFound = await SGDBDownloader.DownloadAndProcessIcoAsync(icon.FullImageUrl,
                        Path.Combine(AppFiles.SavedImagesPath, files["icon"]));
                }
                else
                {
                    result.IconFound = await SGDBDownloader.DownloadImageAsync(icon.FullImageUrl,
                        Path.Combine(AppFiles.SavedImagesPath, files["icon"]),
                        sizeLimits: new[] { 256, 256 });
                }
            }
        }

        return result;
    }
}