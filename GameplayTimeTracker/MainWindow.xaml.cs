using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Gtk;
using static System.Windows.Forms.Cursors;
using Action = System.Action;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Grid = System.Windows.Controls.Grid;
using Int32 = System.Int32;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;

namespace GameplayTimeTracker
{
    public partial class MainWindow : System.Windows.Window
    {
        private const string jsonFilePath = "data.json";
        private const string? SampleImagePath = "assets/no_icon.png";
        private const string? AppIcon = "assets/MyAppIcon.ico";
        private bool isBlurToggled = false;
        private bool isAnimating = false;

        TileContainer tileContainer = new();
        List<Tile> tilesList = new List<Tile>();

        public JsonHandler handler = new();
        ProcessTracker tracker = new();

        List<Theme> themesList = new List<Theme>();
        private Settings settings;

        private NotificationHandler notificationHandler = new();

        private SettingsMenu settingsMenu;
        private DragDropOverlay dragDropOverlay;

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            TotalPlaytimeTextBlock.Text = $"Total Playtime: {tileContainer.GetTotalPlaytimePretty()}";
            tracker.InitializeProcessTracker(tileContainer);
            settingsMenu = new SettingsMenu(ContainerGrid, SettingsGrid, settings);
            UpdateStackPane();
            tileContainer.Total = GamesLoaded;
            GamesLoaded.Text = $"Games managed: {tileContainer.tilesList.Count}";
        }

        private void InitSettings()
        {
            settings = handler.GetSettingsFromFile();
            themesList = settings.ThemeList;
            LoadTheme(settings.SelectedTheme);
        }

        private void LoadTheme(string themeName)
        {
            if (themesList.Count > 0)
            {
                foreach (var theme in themesList)
                {
                    if (theme.ThemeName.Equals(themeName))
                    {
                        SolidColorBrush scb =
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString(theme.Colors["bgColor"]));
                        ScrollViewer.Background = scb;
                        MainStackPanel.Background = scb;
                        Grid.Background = scb;
                        Grid gridFooter = (Grid)FindName("Footer");
                        gridFooter.Background =
                            new SolidColorBrush((Color)ColorConverter.ConvertFromString(theme.Colors["footerColor"]));
                        Utils.SetColors(theme.Colors);
                        return;
                    }
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            notificationHandler = new NotificationHandler();
            InitSettings();
            handler.InitializeContainer(tileContainer);
            dragDropOverlay = new DragDropOverlay();
            DragDropGrid.Children.Add(dragDropOverlay);

            Closing += MainWindow_Closing;
            Loaded += OnLoaded;
            ContentRendered += MainWindow_ContentRendered;
        }

        private void ClickableRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Clicked rect!!");
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            ShowTilesOnCanvas();
        }

        // TODO: Do this properly
        private void UpdateColors()
        {
            Console.WriteLine(" ######### Updating colors!!!");
            InitSettings();
            foreach (var tile in tileContainer.tilesList)
            {
                // tile.InitializeTile();
                tile.UpdateTileColors();
                // TODO: Needs a better approach 
                tile.UpdateImageVars(false);
            }

            ShowTilesOnCanvas();

            Console.WriteLine(" ######### Colors updated!!!");
            Utils.toUpdate = false;
        }

        private async void UpdateStackPane()
        {
            Stopwatch stopwatch = new Stopwatch();
            await Task.Run(() =>
            {
                stopwatch.Start();

                while (true)
                {
                    stopwatch.Restart();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (Utils.toUpdate)
                        {
                            UpdateColors();
                        }

                        tracker.HandleProcesses();
                        RearrangeTiles();

                        TotalPlaytimeTextBlock.Text = $"Total Playtime: {tileContainer.GetTotalPlaytimePretty()}";
                    });


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

        private void AddEntry(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            fileName = fileName.Substring(0, fileName.Length - 4);

            string uniqueFileName = $"{fileName}-{Guid.NewGuid().ToString()}.png";
            // string? iconPath = $"assets/{uniqueFileName}";
            string iconPath = Path.Combine(Utils.SavedIconsPath, uniqueFileName);

            Utils.PrepIcon(filePath, iconPath);
            iconPath = Utils.IsValidImage(iconPath) ? iconPath : SampleImagePath;

            Tile newTile = new Tile(tileContainer, fileName, 0, 0, iconPath, exePath: filePath);
            newTile.Margin = new Thickness(Utils.TileLeftMargin, 5, 0, 5);

            if (!(Path.GetFileName(filePath).Equals("GameplayTimeTracker.exe") ||
                  Path.GetFileName(filePath).Equals("Gameplay Time Tracker.exe")))
            {
                // Closing all opened edit menus and reseting them to avoid graphical glitch
                foreach (var tile in tileContainer.tilesList)
                {
                    if (tile.IsMenuToggled)
                    {
                        tile.ToggleEdit();
                        tile.WasOpened = false;
                    }
                }

                tileContainer.AddTile(newTile, newlyAdded: true);
                tileContainer.ListTiles();
                ShowTilesOnCanvas();
                handler.WriteContentToFile(tileContainer);
                // MessageBox.Show($"Selected file: {filePath}");
            }
        }

        private void AddExecButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                AddEntry(openFileDialog.FileName);
            }
        }

        private void UpdateTileIndexes()
        {
            tileContainer.tilesList = tileContainer.SortedByProperty("IsRunning", false);
            for (int i = 0; i < tileContainer.tilesList.Count; i++)
            {
                tileContainer.tilesList[i].Index = i;
            }
        }

        private void RearrangeTiles()
        {
            UpdateTileIndexes();

            int animationsPending = 0; // Track pending animations

            for (int i = 0; i < tileContainer.tilesList.Count; i++)
            {
                var tile = tileContainer.tilesList[i];
                if (MainStackPanel.Children.Contains(tile))
                {
                    int oldIndex = MainStackPanel.Children.IndexOf(tile);
                    if (tile.Index != oldIndex)
                    {
                        double offset = (tile.Index - oldIndex) * (tile.RenderSize.Height + 10);

                        // Create and configure the animation
                        var animation = new DoubleAnimation
                        {
                            From = 0,
                            To = offset,
                            Duration = TimeSpan.FromSeconds(0.25),
                            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
                        };

                        tile.RenderTransform = new TranslateTransform();
                        TranslateTransform transform = (TranslateTransform)tile.RenderTransform;

                        // Increment pending animations counter
                        animationsPending++;
                        animation.Completed += (s, e) =>
                        {
                            animationsPending--; // Decrement counter

                            // When all animations are completed
                            if (animationsPending == 0)
                            {
                                // Rearrange tiles after all animations are finished
                                Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    for (int j = 0; j < tileContainer.tilesList.Count; j++)
                                    {
                                        var t = tileContainer.tilesList[j];
                                        if (MainStackPanel.Children.Contains(t))
                                        {
                                            MainStackPanel.Children.Remove(t);
                                            MainStackPanel.Children.Insert(t.Index, t);
                                        }

                                        // Reset the transform to clear offset
                                        t.RenderTransform = null;
                                    }
                                }), DispatcherPriority.Background);
                            }
                        };

                        // Start the animation on the tile
                        transform.BeginAnimation(TranslateTransform.YProperty, animation);
                    }
                }
            }
        }

        private void ShowTilesOnCanvas()
        {
            MainStackPanel.Children.Clear();
            foreach (var tile in tileContainer.tilesList)
            {
                tile.Margin = new Thickness(Utils.TileLeftMargin, 5, 0, 5);

                MainStackPanel.Children.Add(tile);
            }
        }

        public void ShowScrollViewerOverlay(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            bool isVerticalScrollVisible = scrollViewer.ExtentHeight > scrollViewer.ViewportHeight;

            OverlayTop.Visibility = e.VerticalOffset > 0 ? Visibility.Visible : Visibility.Collapsed;
            OverlayBottom.Visibility = e.VerticalOffset < ScrollViewer.ScrollableHeight
                ? Visibility.Visible
                : Visibility.Collapsed;
            OverlayTop.Width = scrollViewer.ViewportWidth * 2;
            OverlayBottom.Width = scrollViewer.ViewportWidth * 2;

            double newWidth = isVerticalScrollVisible
                ? Width - 2 * Utils.TileLeftMargin - 2 * SystemParameters.VerticalScrollBarWidth
                : Width - 5 * Utils.TileLeftMargin;
            tileContainer.UpdateTilesWidth(newWidth);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;

                PopupMenu exitPopup = new PopupMenu(
                    text: "Are you sure you want to exit?",
                    routedEvent1: ExitButton_Click);
                exitPopup.OpenMenu();

                // Reinitialize NotifyIcon if it's null
                if (notificationHandler.m_notifyIcon == null)
                {
                    notificationHandler.InitializeNotifyIcon();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                e.Cancel = true; // Cancel closing in case of an error
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            tileContainer?.InitSave();
            // Close the application if Yes is clicked
            Application.Current.Shutdown();
        }

        private void OnCloseNotify(object? sender, CancelEventArgs e)
        {
            notificationHandler.OnCloseNotify(sender, e);
        }

        private void OnStateChanged(object? sender, EventArgs e)
        {
            notificationHandler.OnStateChanged(sender, e);
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            notificationHandler.OnIsVisibleChanged(sender, e);
        }

        private void OpenSettingsWindow(object sender, RoutedEventArgs e)
        {
            // settingsMenu = new SettingsMenu(ContainerGrid);
            settingsMenu.OpenMenu();
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (!isAnimating)
            {
                DragDropGrid.Visibility = Visibility.Visible;
                isAnimating = true; // Prevent further animations while one is in progress

                Utils.dragFadeInAnimation.Completed += (s, o) => { isAnimating = false; };

                DragDropGrid.BeginAnimation(OpacityProperty, Utils.dragFadeInAnimation);
            }

            e.Handled = true; // Marks event as handled
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            if (!isAnimating)
            {
                Utils.dragFadeOutAnimation.Completed += (s, o) =>
                {
                    DragDropGrid.Visibility = Visibility.Collapsed;
                    isAnimating = false; // Allow new animations after this one completes
                };

                DragDropGrid.BeginAnimation(OpacityProperty, Utils.dragFadeOutAnimation);
                isAnimating = true; // Prevent further animations while one is in progress
            }

            e.Handled = true; // Marks event as handled
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (!isAnimating)
            {
                Utils.dragFadeOutAnimation.Completed += (s, o) =>
                {
                    DragDropGrid.Visibility = Visibility.Collapsed;
                    isAnimating = false; // Allow new animations after this one completes
                };

                DragDropGrid.BeginAnimation(OpacityProperty, Utils.dragFadeOutAnimation);
                isAnimating = true; // Prevent further animations while one is in progress
            }

            // Handle the dropped data here (e.g., process the file)
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    AddEntry(file);
                }
            }

            e.Handled = true; // Marks event as handled
        }
    }
}