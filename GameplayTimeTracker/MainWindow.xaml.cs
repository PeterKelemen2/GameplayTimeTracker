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
using static System.Windows.Forms.Cursors;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
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

        TileContainer tileContainer = new();
        List<Tile> tilesList = new List<Tile>();

        public JsonHandler handler = new();
        ProcessTracker tracker = new();

        List<Theme> themesList = new List<Theme>();
        private Settings settings;

        private NotificationHandler notificationHandler = new();

        private SettingsMenu settingsMenu;
        // private System.Windows.Forms.NotifyIcon m_notifyIcon;

        public Grid GetContainerGrid()
        {
            return ContainerGrid;
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            // ShowTilesOnCanvas();
            // tileContainer.ListTiles();
            // handler.WriteContentToFile(tileContainer, jsonFilePath);
            notificationHandler.SetupNotifyIcon();
            TotalPlaytimeTextBlock.Text = $"Total Playtime: {tileContainer.GetTotalPlaytimePretty()}";
            tracker.InitializeProcessTracker(tileContainer);
            UpdateStackPane();
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
            settingsMenu = new SettingsMenu(ContainerGrid);
            settings = handler.GetSettingsFromFile();

            themesList = settings.ThemeList;
            LoadTheme("default");

            handler.InitializeContainer(tileContainer);
            // tilesList = tileContainer.tilesList;

            // CustomButton testButton =
            //     new CustomButton(text: "Test", isBold: true, buttonImagePath: "assets/edit.png");
            // testButton.Margin = new Thickness(250, 0, 0, 0);
            // testButton.Height = 0;
            // Footer.Children.Add(testButton);
            // testButton.Height = 40;
            // testButton.Grid.MouseDown += ClickableRect_MouseDown;

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
                        tracker.HandleProcesses();
                        if (!settingsMenu.ToClose)
                        {
                            settingsMenu.SetBlurImage();
                        }

                        // Call RearrangeTiles only if it's safe to do so
                        if (!isRearranging)
                        {
                            RearrangeTiles();
                        }

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

        private void AddExecButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // Handle the selected file
                string filePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(filePath);
                fileName = fileName.Substring(0, fileName.Length - 4);

                string uniqueFileName = $"{fileName}-{Guid.NewGuid().ToString()}.png";
                string? iconPath = $"assets/{uniqueFileName}";

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
                    // tilesList = tileContainer.GetTiles();
                    tileContainer.ListTiles();
                    ShowTilesOnCanvas();
                    MessageBox.Show($"Selected file: {fileName}");
                }
                else
                {
                    MessageBox.Show("Sorry, I can't keep tabs on myself.", "Existential crisis", MessageBoxButton.OK);
                }
            }

            handler.WriteContentToFile(tileContainer);
        }


        private bool isRearranging = false;

        private void RearrangeTiles()
        {
            if (isRearranging)
                return; // Prevent concurrent execution

            isRearranging = true;

            List<Tile> toMoveList = tileContainer.toMoveList;
            for (int i = 0; i < toMoveList.Count; i++)
            {
                var tileToMove = toMoveList[i];
                if (MainStackPanel.Children.Contains(tileToMove))
                {
                    int oldIndex = MainStackPanel.Children.IndexOf(tileToMove);
                    Console.WriteLine($"{tileToMove.GameName}: oldIndex={oldIndex}, newIndex={i}");
                    if (oldIndex != i)
                    {
                        // Calculate the offset for animation
                        double offset = -i * (tileToMove.RenderSize.Height + 10); // Adjust for spacing

                        var animation = new DoubleAnimation
                        {
                            From = 0,
                            To = offset,
                            Duration = TimeSpan.FromSeconds(0.3),
                            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
                        };

                        tileToMove.RenderTransform = new TranslateTransform();
                        TranslateTransform transform = (TranslateTransform)tileToMove.RenderTransform;
                        tileToMove.WasMoved = true;

                        var i1 = i;
                        animation.Completed += (s, e) =>
                        {
                            // Remove and insert tile at new position
                            MainStackPanel.Children.RemoveAt(oldIndex);
                            MainStackPanel.Children.Insert(i1, tileToMove);
                            Console.WriteLine($"{tileToMove.GameName}: current index: {MainStackPanel.Children.IndexOf(tileToMove)}");
                            // Reset transform after moving to avoid permanent offset
                            transform.Y = 0;
                            tileToMove.RenderTransform = null; // Clear transform for future animations
                            tileToMove.WasMoved = true;
                            Console.WriteLine("######### Animation finished!");
                        };

                        transform.BeginAnimation(TranslateTransform.YProperty, animation);
                    }
                }
            }

            // Reset move list after processing
            tileContainer.ResetMoveList();
            isRearranging = false; // Allow future rearrangements
        }

        private void ShowTilesOnCanvas()
        {
            MainStackPanel.Children.Clear();
            // var tilesList = tileContainer.GetTiles();
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

            double newWidth = isVerticalScrollVisible
                ? Width - 2 * Utils.TileLeftMargin - 2 * SystemParameters.VerticalScrollBarWidth
                : Width - 5 * Utils.TileLeftMargin;
            tileContainer.UpdateTilesWidth(newWidth);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No)
                {
                    e.Cancel = true;

                    // Reinitialize NotifyIcon if it's null
                    if (notificationHandler.m_notifyIcon == null)
                    {
                        notificationHandler.InitializeNotifyIcon();
                    }

                    return;
                }

                // If the user confirmed the exit, proceed with the save logic
                tileContainer?.InitSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                e.Cancel = true; // Cancel closing in case of an error
            }
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
            settingsMenu.OpenSettingsWindow(sender, e);
        }
    }
}