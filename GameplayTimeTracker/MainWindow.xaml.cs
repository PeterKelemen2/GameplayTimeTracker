using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GameplayTimeTracker.Helper;
using GameplayTimeTracker.Menu;
using GameplayTimeTracker.Settings;
using GameplayTimeTracker.SGDB;
using Gtk;
using Shellify;
using Application = System.Windows.Application;
using Grid = System.Windows.Controls.Grid;
using Window = System.Windows.Window;

namespace GameplayTimeTracker;

public partial class MainWindow : Window
{
    private EntryRepository entryRepository;
    private GameCardRepository gameCardRepository;
    private AppTheme TestTheme;
    private double _scrollTarget = 0;
    private double _scrollOffset = 0;
    private const double ScrollSpeed = 80;
    private DragDropOverlay dragDropOverlay;
    private bool isAnimating = false;


    public MainWindow()
    {
        InitializeComponent();
        MainScrollViewer.PreviewMouseWheel += MainScrollViewer_PreviewMouseWheel;
        Common.Settings = DataHandler.GetSettingsFromFile();
        dragDropOverlay = new DragDropOverlay();
        DragDropGrid.Children.Add(dragDropOverlay);

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
        StartCheckingEntries();

        if (Common.Settings.SGDBApiKey.Length == 0 && !Common.Settings.DontShowApiKeyPrompt)
        {
            var sgdbApiKeyPrompt = new PromptMenu(
                width: 400,
                textArray: new[]
                {
                    "You don't have a SteamGridDB API Key set.",
                    "Would you like to set one?",
                    "Clicking this text will open the website to get one.",
                },
                boldArray: new[] { true, true, false },
                lineSpacing: 5, type: PromptMenu.PromptType.YesNo, dontShowAgainQuestion: true,
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
                    Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open link: {ex.Message}");
                }
            };

            sgdbApiKeyPrompt.Open();
        }
    }

    private async void StartCheckingEntries()
    {
        Stopwatch stopwatch = new Stopwatch();
        await Task.Run(() =>
        {
            stopwatch.Start();

            int cycleCount = 0;
            while (true)
            {
                stopwatch.Restart();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    entryRepository.ManageEntriesState();
                    // tracker.HandleProcesses();
                    // RearrangeTiles();
                    cycleCount++;
                });

                if (cycleCount >= Common.Settings.SavingFrequencyInMinutes * 60)
                {
                    DataHandler.WriteEntriesToFile(entryRepository.EntriesList, AppFiles.DataFilePath);
                    cycleCount = 0;
                }

                stopwatch.Stop();
                Console.WriteLine($"Cycle took {stopwatch.Elapsed.TotalMilliseconds.ToString("F2")}ms");

                if ((int)stopwatch.ElapsedMilliseconds < 1000)
                {
                    Task.Delay(1000 - (int)stopwatch.ElapsedMilliseconds).Wait();
                }
                // Task.Delay(10).Wait();
            }
        });
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
        gameCardRepository.LoadCards(entryRepository, MainPanel);
    }

    private void SetBaseColorBindings()
    {
        BindingHelper.SetColorBinding(Footer, BackgroundProperty, "Footer");
        BindingHelper.SetColorBinding(MainScrollViewer, BackgroundProperty, "Background");
        BindingHelper.SetColorBinding(GamesLoadedBlock, ForegroundProperty, "Footer Font");
        BindingHelper.SetColorBinding(TotalPlaytimeTextBlock, ForegroundProperty, "Footer Font");
        BindingHelper.SetGradientColorBinding(OverlayTop, Shape.FillProperty, "Background", "Transparent");
        BindingHelper.SetGradientColorBinding(OverlayBottom, Shape.FillProperty, "Transparent", "Background");
    }

    private void SetUpFooter()
    {
        CustomButton AddButton = new CustomButton(w: 40, h: 40, hA: HorizontalAlignment.Left,
            bImgPath: AppFiles.AddIcon, effect: AppEffects.DropShadowIcon);
        AddButton.Margin = new Thickness(15, 0, 0, 0);
        AddButton.Click += (_, _) =>
        {
            string path = Common.GetDialogPath(Common.exeFilter);
            EntryController.AddEntry(path, entryRepository, gameCardRepository, MainPanel);
        };
        Grid.SetRow(AddButton, 1);
        MainGrid.Children.Add(AddButton);

        CustomButton SettingsButton = new CustomButton(w: 40, h: 40, hA: HorizontalAlignment.Left,
            bImgPath: AppFiles.CogIcon, effect: AppEffects.DropShadowIcon);
        SettingsButton.Margin = new Thickness(70, 0, 0, 0);
        SettingsButton.Click += (_, _) =>
        {
            SettingsMenu settingsMenu = new SettingsMenu();
            settingsMenu.Open();
        };
        Grid.SetRow(SettingsButton, 1);
        MainGrid.Children.Add(SettingsButton);

        GamesLoadedBlock.Effect = AppEffects.DropShadowIcon;
        TotalPlaytimeTextBlock.Effect = AppEffects.DropShadowIcon;
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

        OverlayBottom.Width = grid.ActualWidth;
        OverlayTop.Width = grid.ActualWidth;
    }

    private void MainScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        double verticalOffset = e.VerticalOffset; // Current scroll position
        double scrollableHeight = e.ExtentHeight - e.ViewportHeight;

        OverlayTop.Visibility = verticalOffset < 10 ? Visibility.Collapsed : Visibility.Visible;
        OverlayBottom.Visibility = verticalOffset < scrollableHeight - 10 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;
        _scrollTarget -= e.Delta > 0 ? ScrollSpeed : -ScrollSpeed;
        _scrollTarget = Math.Max(0, Math.Min(MainScrollViewer.ScrollableHeight, _scrollTarget));

        SmoothScrollTo(_scrollTarget);
    }

    private void SmoothScrollTo(double toValue)
    {
        // Stop any existing animation
        MainScrollViewer.BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, null);

        DoubleAnimation animation = new DoubleAnimation
        {
            From = MainScrollViewer.VerticalOffset,
            To = toValue,
            Duration = TimeSpan.FromMilliseconds(Common.ScrollDurationsMs),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        animation.Completed += (s, e) => MainScrollViewer.ScrollToVerticalOffset(toValue);

        MainScrollViewer.BeginAnimation(ScrollViewerBehavior.VerticalOffsetProperty, animation);
    }

    private void Grid_DragEnter(object sender, DragEventArgs e)
    {
        if (!isAnimating)
        {
            DragDropGrid.Visibility = Visibility.Visible;
            isAnimating = true; // Prevent further animations while one is in progress

            AppAnimations.DragFadeIn.Completed += (s, o) => { isAnimating = false; };
            DragDropGrid.BeginAnimation(OpacityProperty, AppAnimations.DragFadeIn);
        }

        e.Handled = true; // Marks event as handled
    }

    private void Grid_DragLeave(object sender, DragEventArgs e)
    {
        if (!isAnimating)
        {
            AppAnimations.DragFadeOut.Completed += (s, o) =>
            {
                DragDropGrid.Visibility = Visibility.Collapsed;
                isAnimating = false; // Allow new animations after this one completes
            };

            DragDropGrid.BeginAnimation(OpacityProperty, AppAnimations.DragFadeOut);
            isAnimating = true; // Prevent further animations while one is in progress
        }

        e.Handled = true; // Marks event as handled
    }

    private void Grid_Drop(object sender, DragEventArgs e)
    {
        // Handle the dropped data here (e.g., process the file)
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                EntryController.AddEntry(file, entryRepository, gameCardRepository, MainPanel);
            }
        }

        if (!isAnimating)
        {
            AppAnimations.DragFadeOut.Completed += (s, o) =>
            {
                DragDropGrid.Visibility = Visibility.Collapsed;
                isAnimating = false; // Allow new animations after this one completes
            };

            DragDropGrid.BeginAnimation(OpacityProperty, AppAnimations.DragFadeOut);
            isAnimating = true; // Prevent further animations while one is in progress
        }

        e.Handled = true; // Marks event as handled
    }
}