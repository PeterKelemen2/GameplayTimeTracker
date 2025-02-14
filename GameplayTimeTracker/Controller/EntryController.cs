using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using GameplayTimeTracker.Menu;
using GameplayTimeTracker.SGDB;
using Shellify;

namespace GameplayTimeTracker;

public static class EntryController
{
    public static void AddEntry(string path, EntryRepository repository, GameCardRepository cardRepository,
        Panel parentPanel)
    {
        string arguments = "";
        string exePath = "";

        if (Path.GetExtension(path).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
        {
            var shortcut = ShellLinkFile.Load(path);
            exePath += shortcut.LinkInfo.LocalBasePath;
            arguments += shortcut.Arguments;
        }
        else if (Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase))
        {
            exePath += path;
        }

        if (!exePath.Equals(""))
        {
            if (!repository.IsExePresent(exePath))
            {
                Entry newEntry = new Entry();
                newEntry.ExePath = exePath;
                newEntry.Arguments = arguments;
                string name = FileVersionInfo.GetVersionInfo(newEntry.ExePath).FileDescription;
                name = string.IsNullOrEmpty(name) ? Path.GetFileNameWithoutExtension(newEntry.ExePath) : name;
                name = name.Trim();
                newEntry.Name = name;

                if (Common.Settings.PreferSteamGridDBImage)
                {
                    if (Common.Settings.SGDBApiKey.Length == 0 ||
                        !Common.Settings.SGDBApiKey.Equals(Common.NoApiKeyText))
                    {
                        _ = HandleSGDBImages(newEntry);
                    }
                    else
                    {
                        var sgdbApiKeyPrompt = new PromptMenu(
                            width: 350,
                            textArray: new[]
                                { "You don't have a SteamGridDB API Key set.", "Local icon image was used.", },
                            boldArray: new[] { true, false }, lineSpacing: 5, type: PromptMenu.PromptType.Ok
                        );
                        sgdbApiKeyPrompt.Open();
                        HandleLocalImages(newEntry);
                    }
                }
                else
                {
                    HandleLocalImages(newEntry);
                }

                repository.AddEntry(newEntry);

                if (!Common.Settings.QuickAdd)
                {
                    EntryConfigMenu configMenu = new EntryConfigMenu(newEntry);
                    configMenu.Open();
                }
            }
            else
            {
                PromptMenu duplicatePrompt =
                    new PromptMenu(
                        width: 400,
                        textArray: new[]
                        {
                            "Sorry, this executable is already in use by",
                            repository.GetNameByExePath(exePath),
                            "Would you like to select another file?"
                        },
                        sizeArray: new[] { Common.EditTitleFontSize, Common.EditTitleFontSize + 2 },
                        boldArray: new[] { false, true },
                        lineSpacing: 5,
                        type: PromptMenu.PromptType.YesNo,
                        yesHandler: (s, e) =>
                        {
                            string newpath = Common.GetDialogPath(Common.exeFilter);
                            AddEntry(newpath, repository, cardRepository, parentPanel);
                        }
                    );
                duplicatePrompt.Open();
            }
        }
    }

    public static async Task HandleSGDBImages(Entry entry)
    {
        try
        {
            EventPopup SGDBfetch = new EventPopup("Started loading from SGDB!");
            Dictionary<string, string> iconFiles = SGDBFileHandler.GetSGDBFiles(entry.Name);

            var fetchResult = await SGDBFetch.FetchSGDBAsync(Common.Settings.SGDBApiKey, entry.Name, iconFiles);

            if (fetchResult.GameFound)
            {
                if (fetchResult.HeroFound && File.Exists(iconFiles["hero"])) entry.HeroPath = iconFiles["hero"];
                if (fetchResult.IconFound && File.Exists(iconFiles["icon"])) entry.IconPath = iconFiles["icon"];

                EventPopup gameFound = new EventPopup("Game found on SteamGridDB!");
                if (fetchResult.IconFound && fetchResult.HeroFound)
                {
                    EventPopup bothImagesFound = new EventPopup("Icon and Hero images found!");
                }
            }
            else
            {
                EventPopup gameNotFound = new EventPopup("Couldn't find Game on SGDB.", EventType.Negative);
            }
        }
        catch (Exception ex)
        {
            // HandleLocalImages(entry);
            Console.WriteLine(ex);
        }
    }

    public static void HandleLocalImages(Entry entry)
    {
        EventPopup localFetch = new EventPopup("Started loading local images!");

        RefreshLocalIcon(entry);
        RefreshLocalHero(entry);
    }

    public static async void RefreshLocalHero(Entry entry, string fileName = "")
    {
        await Task.Run(() =>
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Guid guid = Guid.NewGuid();
                fileName = $"local_refresh_{entry.Name}_{guid}";
            }

            string newImagePath = Path.Combine(AppFiles.SavedImagesPath, $"{fileName}_hero.png");
            ImageHelper.ScatterImage(entry.IconPath, newImagePath);

            entry.HeroPath = newImagePath;
            Application.Current.Dispatcher.Invoke(() =>
            {
                EventPopup localHero = new EventPopup("Loaded local Hero!");
            });
        });
    }

    public static async void RefreshLocalIcon(Entry entry, string fileName = "")
    {
        await Task.Run(() =>
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Guid guid = Guid.NewGuid();
                fileName = $"local_refresh_{entry.Name}_{guid}";
            }

            string newImagePath = Path.Combine(AppFiles.SavedImagesPath, $"{fileName}_icon.png");
            ImageHelper.SaveIconFromExe(entry.ExePath, newImagePath);
            entry.IconPath = newImagePath;
            Application.Current.Dispatcher.Invoke(() =>
            {
                EventPopup localIcon = new EventPopup($"Loaded local Icon for {entry.Name} !");
            });
        });
    }
}