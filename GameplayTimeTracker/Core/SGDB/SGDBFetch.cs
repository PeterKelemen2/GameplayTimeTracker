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
                await SGDBDownloader.DownloadImageAsync(hero.FullImageUrl,
                    Path.Combine(AppFiles.SavedImagesPath, files["hero"]),
                    sizeLimits: new[] { 960, 310 });
                EventPopup heroOk = new EventPopup("Hero found!");
                result.HeroFound = true;
            }
            else
            {
                EventPopup heroNotOk = new EventPopup("Couldn't find Hero.", EventType.Negative);
            }

            var icons = await sgdb.GetIconsByGameIdAsync(game.Id);
            var icon = icons?.FirstOrDefault();
            if (icon != null)
            {
                bool iconResult;
                if (icon.Format == SteamGridDbFormats.Ico)
                {
                    iconResult = await SGDBDownloader.DownloadAndProcessIcoAsync(icon.FullImageUrl,
                        Path.Combine(AppFiles.SavedImagesPath, files["icon"]));
                }
                else
                {
                    iconResult = await SGDBDownloader.DownloadImageAsync(icon.FullImageUrl,
                        Path.Combine(AppFiles.SavedImagesPath, files["icon"]),
                        sizeLimits: new[] { 256, 256 });
                }

                result.IconFound = iconResult;
                EventPopup iconOk = new EventPopup("Icon found!");
            }
            else
            {
                EventPopup iconNotOk = new EventPopup("Couldn't find Icon.", EventType.Negative);
            }
        }
        
        return result;
    }
}