using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using GameplayTimeTracker.Menu;
using GameplayTimeTracker.SGDB;
using Shellify;

namespace GameplayTimeTracker;

public static class EntryController
{
    public static void AddEntry(EntryRepository repository, GameCardRepository cardRepository, Panel parentPanel)
    {
        string arguments = "";
        string exePath = "";
        string path = Common.GetDialogPath(Common.exeFilter);

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
                newEntry.Repository = repository;
                newEntry.ExePath = exePath;
                newEntry.Arguments = arguments;
                string name = FileVersionInfo.GetVersionInfo(newEntry.ExePath).FileDescription;
                name = string.IsNullOrEmpty(name) ? Path.GetFileNameWithoutExtension(newEntry.ExePath) : name;
                newEntry.Name = name;

                Dictionary<string, string> iconFiles = SGDBFileHandler.GetSGDBFiles();

                if (!Common.Settings.SGDBApiKey.Equals(string.Empty))
                {
                    Task.Run(async () =>
                            await SGDBFetch.FetchSGDBAsync(Common.Settings.SGDBApiKey, newEntry.Name, iconFiles))
                        .Wait();
                }

                newEntry.IconPath = iconFiles["icon"];
                newEntry.HeroPath = iconFiles["hero"];

                repository.AddEntry(newEntry);
                GameCard gc = new GameCard();

                switch (Common.Settings.Display)
                {
                    case GameDisplay.Horizontal:
                        gc = new GameCardHorizontal(newEntry, repository, cardRepository, parentPanel);
                        break;
                    case GameDisplay.Vertical:
                        gc = new GameCardVertical(newEntry, repository, cardRepository, parentPanel);
                        break;
                }

                cardRepository.GameCards.Add(gc);
                parentPanel.Children.Add(gc);

                DataHandler.WriteEntriesToFile(repository.EntriesList, AppFiles.DataFilePath);

                if (!Common.Settings.QuickAdd)
                {
                    EntryConfigMenu configMenu = new EditMenu(newEntry);
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
                        yesHandler: (s, e) => { AddEntry(repository, cardRepository, parentPanel); }
                    );
                duplicatePrompt.Open();
            }
        }
    }
}