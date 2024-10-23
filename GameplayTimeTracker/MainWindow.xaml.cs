using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace GameplayTimeTracker
{
    public partial class MainWindow : System.Windows.Window
    {
        private const string jsonFilePath = "data.json";
        private const string? SampleImagePath = "assets/no_icon.png";
        private const string? AppIcon = "assets/MyAppIcon.ico";

        TileContainer tileContainer = new();

        public JsonHandler handler = new();
        ProcessTracker tracker = new();

        List<Theme> themesList = new List<Theme>();

        private System.Windows.Forms.NotifyIcon m_notifyIcon;

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            // ShowTilesOnCanvas();
            // tileContainer.ListTiles();
            // handler.WriteContentToFile(tileContainer, jsonFilePath);
            SetupNotifyIcon();
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
                        Utils.SetColors(theme.Colors);
                        return;
                    }
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            handler.InitializeSettings();

            themesList = handler.GetThemesFromFile();
            LoadTheme("default");

            handler.InitializeContainer(tileContainer, jsonFilePath);


            Closing += MainWindow_Closing;
            Loaded += OnLoaded;
            ContentRendered += MainWindow_ContentRendered;
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
                        var sortedList = tileContainer.SortedByProperty("IsRunning", false);
                        if (!tileContainer.IsListEqual(sortedList))
                        {
                            tileContainer.SetTilesList(sortedList);
                            ShowTilesOnCanvas();
                        }

                        TotalPlaytimeTextBlock.Text = $"Total Playtime: {tileContainer.GetTotalPlaytimePretty()}";
                    });
                    stopwatch.Stop();
                    Console.WriteLine($"Cycle took {stopwatch.Elapsed}ms");
                    if ((int)stopwatch.ElapsedMilliseconds > 1000)
                    {
                        Task.Delay(1000).Wait();
                    }
                    else
                    {
                        Task.Delay(1000 - (int)stopwatch.ElapsedMilliseconds).Wait();
                    }
                    // Task.Delay(10).Wait();
                }
            });
        }

        // ==== Tray ====
        void SetupNotifyIcon()
        {
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "The app has been minimised. Click the tray icon to show.";
            m_notifyIcon.BalloonTipTitle = "Gameplay Time Tracker";
            m_notifyIcon.Text = "Gameplay Time Tracker";
            m_notifyIcon.Icon =
                new System.Drawing.Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppIcon));
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
        }

        void OnCloseNotify(object sender, CancelEventArgs args)
        {
            // Only dispose if you are exiting
            if (!args.Cancel)
            {
                m_notifyIcon.Dispose();
                m_notifyIcon = null;
            }
        }

        private WindowState m_storedWindowState = WindowState.Normal;

        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (m_notifyIcon != null)
                    m_notifyIcon.ShowBalloonTip(2000);
            }
            else
                m_storedWindowState = WindowState;
        }

        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = !IsVisible;
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }

        // ==== Tray ====

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

                string uniqueFileName = Guid.NewGuid().ToString() + ".png";
                string? iconPath = $"assets/{uniqueFileName}";

                Utils.PrepIcon(filePath, iconPath);
                iconPath = Utils.IsValidImage(iconPath) ? iconPath : SampleImagePath;

                Tile newTile = new Tile(tileContainer, fileName, 0, 0, iconPath, exePath: filePath);
                newTile.Margin = new Thickness(Utils.TileLeftMargin, 5, 0, 5);

                if (!(Path.GetFileName(filePath).Equals("GameplayTimeTracker.exe") ||
                      Path.GetFileName(filePath).Equals("Gameplay Time Tracker.exe")))
                {
                    // Closing all opened edit menus and reseting them to avoid graphical glitch
                    foreach (var tile in tileContainer.GetTiles())
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
                    MessageBox.Show($"Selected file: {fileName}");
                }
                else
                {
                    MessageBox.Show("Sorry, I can't keep tabs on myself.", "Existential crisis", MessageBoxButton.OK);
                }
            }

            handler.WriteContentToFile(tileContainer, jsonFilePath);
        }

        private void ShowTilesOnCanvas()
        {
            MainStackPanel.Children.Clear();
            var tilesList = tileContainer.GetTiles();
            foreach (var tile in tilesList)
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
                    e.Cancel = true; // Cancel closing
                    // Reinitialize NotifyIcon if it's null
                    if (m_notifyIcon == null)
                    {
                        InitializeNotifyIcon(); // Custom method to reinitialize the NotifyIcon
                    }

                    return; // Exit the method
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

        private void InitializeNotifyIcon()
        {
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "The app has been minimized. Click the tray icon to show.";
            m_notifyIcon.BalloonTipTitle = "Gameplay Time Tracker";
            m_notifyIcon.Text = "Gameplay Time Tracker";
            m_notifyIcon.Icon =
                new System.Drawing.Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppIcon));
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
            m_notifyIcon.Visible = true; // Make sure it's visible
        }
    }
}