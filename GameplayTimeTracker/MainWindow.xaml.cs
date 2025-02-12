using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GameplayTimeTracker.Helper;
using GameplayTimeTracker.Menu;
using GameplayTimeTracker.Settings;
using Hardcodet.Wpf.TaskbarNotification;
using Application = System.Windows.Application;
using Grid = System.Windows.Controls.Grid;
using Window = System.Windows.Window;

namespace GameplayTimeTracker;

public partial class MainWindow : Window
{
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
        AppFiles.EnsureAppFolders();
        dragDropOverlay = new DragDropOverlay();
        DragDropGrid.Children.Add(dragDropOverlay);

        Closing += MainWindow_Closing;
        StateChanged += Window_StateChanged;
        MainGrid.SizeChanged += MainGrid_SizeChanged;
        Loaded += OnLoaded;
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        Root.Focusable = true;
        SetBaseColorBindings();
        LoadAndShowData();
        SetUpFooter();
        StartCheckingEntries();

        Common.TaskbarIcon = (TaskbarIcon)FindResource("AppTaskbarIcon");
        Common.TaskbarIcon.Visibility = Visibility.Visible;
        TaskbarManager.UpdateTrayToolTip();
        TaskbarManager.UpdateTrayEntries();

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
        await Task.Run(async () =>
        {
            stopwatch.Start();

            int cycleCount = 0;
            while (true)
            {
                stopwatch.Restart();

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Common.Repository.ManageEntriesState();
                    cycleCount++;
                }));

                // Periodically save to file
                if (cycleCount >= Common.Settings.SavingFrequencyInMinutes * 60)
                {
                    DataHandler.WriteEntriesToFile(Common.Repository.EntriesList, AppFiles.DataFilePath);
                    cycleCount = 0;
                }

                stopwatch.Stop();
                double cycleTime = stopwatch.Elapsed.TotalMilliseconds;
                double remainingTime = 1000 - cycleTime;

                if (remainingTime > 0) await Task.Delay((int)remainingTime);

                Console.WriteLine($"Cycle took {cycleTime.ToString("F2")}ms, remaining time: {remainingTime:F2}ms");
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
        Common.Repository = new EntryRepository();
        Common.Repository.TotalRuntime = Common.Repository.GetTotalTimeArrays();
        Common.CardRepository = new GameCardRepository();
    }

    public void ShowCards()
    {
        Common.CardRepository.LoadCards(Common.Repository, MainPanel);
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
            EntryController.AddEntry(path, Common.Repository, Common.CardRepository, MainPanel);
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
        Binding managedCountBinding = new Binding("TotalEntryCount")
        {
            Source = Common.Repository,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(GameCountRun, Run.TextProperty, managedCountBinding);

        TotalPlaytimeTextBlock.Effect = AppEffects.DropShadowIcon;
        Binding totalPlaytimeBinding = new Binding("TotalRuntimeFormatted")
        {
            Source = Common.Repository,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(TotalTimeRun, Run.TextProperty, totalPlaytimeBinding);
    }

    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            e.Cancel = true;

            var exitPrompt = new PromptMenu(
                width: 300,
                textArray: new[]
                {
                    "Would you really like to exit?",
                },
                boldArray: new[] { true, },
                type: PromptMenu.PromptType.YesNo,
                yesHandler: ExitButton_YesClick
                // noHandler: (s, e) => { Console.WriteLine("Closing canceled."); }
            );
            exitPrompt.Open();
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
            e.Cancel = true;
        }
    }

    private void ExitButton_YesClick(object sender, RoutedEventArgs e)
    {
        Common.Repository.EntriesList
            .Where(entry => entry.IsRunning)
            .ToList()
            .ForEach(entry => entry.IncrementPlaytimeHistory(toSave: false));

        DataHandler.WriteEntriesToFile(Common.Repository.EntriesList, AppFiles.DataFilePath);

        if (Common.Settings.BackupOnExit) DataHandler.CreateBackup();
        Application.Current.Shutdown();
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
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (!files.All(file =>
                    System.IO.Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase)))
                return;

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

        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                EntryController.AddEntry(file, Common.Repository, Common.CardRepository, MainPanel);
            }
        }

        e.Handled = true; // Marks event as handled
    }


    public void TrayMenu_Open_Click(object sender, RoutedEventArgs e)
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    public void TrayMenu_Exit_Click(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Tray menu exit click");
        ExitButton_YesClick(sender, e);
    }

    private async void Window_StateChanged(object sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            await Task.Delay(200);
            Hide();
            Root.Visibility = Visibility.Collapsed;
        }
        else if (WindowState == WindowState.Normal)
        {
            Root.Visibility = Visibility.Visible;
            Show();
        }
    }
}