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
    private AppSettings Settings;
    private AppTheme TestTheme;

    public MainWindow()
    {
        InitializeComponent();
        Settings = DataHandler.GetSettingsFromFile();
        DataHandler.ManageStartupShortcut(Settings.StartWithSystem);
        foreach (var color in Settings.CurrentTheme.Colors)
        {
            Console.WriteLine($"Color: {color.Key}, {color.Value}");
        }

        MainGrid.SizeChanged += MainGrid_SizeChanged;
        Loaded += OnLoaded;
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        CreateFooterButtons();
        SetBaseColors();
        entryRepository = new EntryRepository();
        gameCardRepository = new GameCardRepository();

        foreach (var entry in entryRepository.EntriesList)
        {
            // GameCard gc = new GameCardHorizontal(entry, entryRepository, gameCardRepository, MainPanel, Settings);
            GameCard gc = new GameCardVertical(entry, entryRepository, gameCardRepository, MainPanel, Settings);
            gameCardRepository.GameCards.Add(gc);
            MainPanel.Children.Add(gc);
        }
    }

    private void SetBaseColors()
    {
        Binding footerBinding = new Binding
        {
            Source = Settings.CurrentTheme.Colors,
            Path = new PropertyPath("[Footer]"),
            Converter = new ColorDictionaryToBrushConverter(),
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(Footer, BackgroundProperty, footerBinding);

        // Settings.PropertyChanged += (sender, args) =>
        // {
        //     if (args.PropertyName == nameof(Settings.CurrentTheme))
        //     {
        //         Binding newBinding = new Binding
        //         {
        //             Source = Settings.CurrentTheme.Colors,
        //             Path = new PropertyPath("[Footer]"),
        //             Converter = new ColorDictionaryToBrushConverter(),
        //             Mode = BindingMode.OneWay,
        //         };
        //         BindingOperations.SetBinding(Footer, BackgroundProperty, newBinding);
        //     }
        // };


        // Console.WriteLine($"Current theme: {Settings.CurrentTheme.ThemeName}");
        // Settings.CurrentTheme = Settings.ThemesList[1];
        // Console.WriteLine($"Current theme: {Settings.CurrentTheme.ThemeName}");

        // Settings.CurrentTheme.UpdateColor("Footer", "#3BC9E3");

        MainScrollViewer.Background = new SolidColorBrush(AppColors.Background);
    }

    private void CreateFooterButtons()
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
            SettingsMenu settingsMenu = new SettingsMenu(Settings);
            settingsMenu.Open();
        };
        Grid.SetRow(SettingsButton, 1);
        MainGrid.Children.Add(SettingsButton);
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
                    new AddMenu(entry: newEntry, entryRepository, gameCardRepository, MainPanel, Settings);
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