using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GameplayTimeTracker.Menu;
using GameplayTimeTracker.Settings;
using GameplayTimeTracker.SGDB;
using Shellify;

namespace GameplayTimeTracker;

public partial class MainWindow : Window
{
    private EntryRepository entryRepository;

    private GameCardRepository gameCardRepository;

    // public AppSettings Settings;
    private AppTheme TestTheme;

    public MainWindow()
    {
        InitializeComponent();
        Common.Settings = DataHandler.GetSettingsFromFile();
        // Settings = Common.Settings;
        DataHandler.ManageStartupShortcut(Common.Settings.StartWithSystem);
        foreach (var color in Common.Settings.CurrentTheme.Colors)
        {
            Console.WriteLine($"Color: {color.Key}, {color.Value}");
        }

        MainGrid.SizeChanged += MainGrid_SizeChanged;
        Loaded += OnLoaded;
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        SetUpFooter();
        SetBaseColorBindings();

        LoadAndShowData();

        if (Common.Settings.SGDBApiKey.Length == 0)
        {
            var sgdbApiKeyPrompt = new PromptMenu(
                // height: 200,
                width: 400,
                textArray: new[]
                {
                    "You don't have a SteamGridDB API Key set.",
                    "Would you like to set one?",
                    "Clicking this text will open the website to get one.",
                },
                // sizeArray: new[] { },
                boldArray: new[] { true, true, false },
                lineSpacing: 5,
                type: PromptMenu.PromptType.YesNo,
                yesHandler: (s, e) =>
                {
                    SettingsMenu settingsMenu = new SettingsMenu();
                    settingsMenu.Open();
                }
            );
            sgdbApiKeyPrompt.promptTextBlock.MouseDown += (s, e) =>
            {
                string url = "https://www.steamgriddb.com/profile/preferences/api";
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true // Required for launching URLs in default browser
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open link: {ex.Message}");
                }
            };
            sgdbApiKeyPrompt.Open();
        }
    }

    public void LoadAndShowData()
    {
        LoadData();
        ShowCards();
    }

    public void LoadData()
    {
        entryRepository = new EntryRepository();
        gameCardRepository = new GameCardRepository();
        GameCountRun.Text = entryRepository.EntriesList.Count.ToString();
        TotalTimeRun.Text = Common.GetPrettyTimeFromDouble(entryRepository.GetTotalTime());
    }

    public void ShowCards()
    {
        MainPanel.Children.Clear();
        foreach (var entry in entryRepository.EntriesList)
        {
            GameCard gc = new GameCard();
            switch (Common.Settings.Display)
            {
                case GameDisplay.Vertical:
                    gc = new GameCardVertical(entry, entryRepository, gameCardRepository, MainPanel);
                    break;
                case GameDisplay.Horizontal:
                    gc = new GameCardHorizontal(entry, entryRepository, gameCardRepository, MainPanel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // GameCard gc = new GameCardHorizontal(entry, entryRepository, gameCardRepository, MainPanel, Settings);
            // GameCard gc = new GameCardVertical(Settings, entry, entryRepository, gameCardRepository, MainPanel);
            gameCardRepository.GameCards.Add(gc);
            MainPanel.Children.Add(gc);
        }
    }

    private void SetBaseColorBindings()
    {
        BindingHelper.SetColorBinding(Footer, BackgroundProperty, "Footer");
        BindingHelper.SetColorBinding(MainScrollViewer, BackgroundProperty, "Background");
        BindingHelper.SetColorBinding(GamesLoadedBlock, ForegroundProperty, "Footer Font");
        BindingHelper.SetColorBinding(TotalPlaytimeTextBlock, ForegroundProperty, "Footer Font");
    }

    private void SetUpFooter()
    {
        CustomButton AddButton = new CustomButton(w: 40, h: 40, hA: HorizontalAlignment.Left,
            bImgPath: AppFiles.AddIcon, effect: AppEffects.dropShadowIcon);
        AddButton.Margin = new Thickness(15, 0, 0, 0);
        AddButton.Click += (_, _) => { AddEntry(); };
        Grid.SetRow(AddButton, 1);
        MainGrid.Children.Add(AddButton);

        CustomButton SettingsButton = new CustomButton(w: 40, h: 40, hA: HorizontalAlignment.Left,
            bImgPath: AppFiles.CogIcon, effect: AppEffects.dropShadowIcon);
        SettingsButton.Margin = new Thickness(70, 0, 0, 0);
        SettingsButton.Click += (_, _) =>
        {
            SettingsMenu settingsMenu = new SettingsMenu();
            settingsMenu.Open();
        };
        Grid.SetRow(SettingsButton, 1);
        MainGrid.Children.Add(SettingsButton);

        GamesLoadedBlock.Effect = AppEffects.dropShadowIcon;
        TotalPlaytimeTextBlock.Effect = AppEffects.dropShadowIcon;
    }

    public void AddEntry()
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
            if (!entryRepository.IsExePresent(exePath))
            {
                Entry newEntry = new Entry();
                newEntry.Repository = entryRepository;
                newEntry.ExePath = exePath;
                newEntry.Arguments = arguments;
                string name = FileVersionInfo.GetVersionInfo(newEntry.ExePath).FileDescription;
                name = string.IsNullOrEmpty(name) ? Path.GetFileNameWithoutExtension(newEntry.ExePath) : name;
                newEntry.Name = name;

                CustomMenu addEntryConfigMenu =
                    new AddMenu(newEntry, entryRepository, gameCardRepository, MainPanel);
                addEntryConfigMenu.Open();
            }
            else
            {
                PromptMenu duplicatePrompt =
                    new PromptMenu(
                        // height: 200,
                        width: 400,
                        textArray: new[]
                        {
                            "Sorry, this executable is already in use by",
                            entryRepository.GetNameByExePath(exePath),
                            "Would you like to select another file?"
                        },
                        sizeArray: new[] { Common.EditTitleFontSize, Common.EditTitleFontSize + 2 },
                        boldArray: new[] { false, true },
                        lineSpacing: 5,
                        type: PromptMenu.PromptType.YesNo,
                        yesHandler: (s, e) => { AddEntry(); }
                    );
                duplicatePrompt.Open();
            }
        }
    }

    private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var grid = sender as Grid;
        var scaleTransform = grid.RenderTransform as ScaleTransform;

        if (scaleTransform != null)
        {
            scaleTransform.CenterX = grid.ActualWidth / 2;
            scaleTransform.CenterY = grid.ActualHeight / 2;
        }
    }
}