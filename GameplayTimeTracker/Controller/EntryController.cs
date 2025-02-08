using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                    if (Common.Settings.SGDBApiKey.Length > 0)
                    {
                        HandleSGDBImages(newEntry);
                    }
                    else
                    {
                        var sgdbApiKeyPrompt = new PromptMenu(
                            width: 400,
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

    private static void HandleSGDBImages(Entry entry)
    {
        Dictionary<string, string> iconFiles = SGDBFileHandler.GetSGDBFiles(entry.Name);

        if (!Common.Settings.SGDBApiKey.Equals(string.Empty))
        {
            Task.Run(async () =>
                    await SGDBFetch.FetchSGDBAsync(Common.Settings.SGDBApiKey, entry.Name, iconFiles))
                .Wait();
        }

        entry.IconPath = iconFiles["icon"];
        entry.HeroPath = iconFiles["hero"];
    }

    public static void HandleLocalImages(Entry entry)
    {
        Guid guid = Guid.NewGuid();
        string iconPath = Path.Combine(AppFiles.SavedImagesPath,
            $"{entry.Name.Replace(" ", "_")}_{guid.ToString()}_icon.png");
        string heroPath = Path.Combine(AppFiles.SavedImagesPath,
            $"{entry.Name.Replace(" ", "_")}_{guid.ToString()}_hero.png");

        ImageHelper.SaveIconFromExe(entry.ExePath, iconPath);
        entry.IconPath = iconPath;
        ImageHelper.ScatterImage(iconPath, heroPath);
        entry.HeroPath = heroPath;
    }
}