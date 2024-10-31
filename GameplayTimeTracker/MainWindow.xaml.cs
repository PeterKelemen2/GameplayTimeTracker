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
            handler.InitializeSettings();

            themesList = handler.GetThemesFromFile();
            LoadTheme("default");

            handler.InitializeContainer(tileContainer, jsonFilePath);
            tilesList = tileContainer.GetTiles();


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
                        settingsMenu.SetBlurImage();
                        
                        var sortedList = tileContainer.SortedByProperty("IsRunning", false);
                        if (!tileContainer.IsListEqual(sortedList))
                        {
                            tileContainer.SetTilesList(sortedList);
                            ShowTilesOnCanvas();
                        }

                        TotalPlaytimeTextBlock.Text = $"Total Playtime: {tileContainer.GetTotalPlaytimePretty()}";
                    });
                    stopwatch.Stop();
                    Console.WriteLine($"Cycle took {stopwatch.Elapsed.TotalMilliseconds.ToString("F2")}ms");

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
                    foreach (var tile in tileContainer.GetTiles())
                    {
                        if (tile.IsMenuToggled)
                        {
                            tile.ToggleEdit();
                            tile.WasOpened = false;
                        }
                    }

                    tileContainer.AddTile(newTile, newlyAdded: true);
                    tilesList = tileContainer.GetTiles();
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


        // private void CreateBlurRectangle()
        // {
        //     // Create a new Rectangle for the blur overlay
        //     Rectangle blurOverlay = new Rectangle
        //     {
        //         Width = ContainerGrid.ActualWidth,
        //         Height = ContainerGrid.ActualHeight,
        //         Fill = new SolidColorBrush(Colors.Black),
        //         // Opacity = 0.8,
        //         // IsHitTestVisible = false // Make the overlay non-clickable
        //     };
        //
        //     // Set attached properties
        //     Panel.SetZIndex(blurOverlay, 0); // Ensure it appears above other elements
        //     Grid.SetRow(blurOverlay, 0); // Set to the first row of the Grid
        //
        //     // Create a VisualBrush to capture visuals behind the rectangle
        //     VisualBrush visualBrush = new VisualBrush();
        //
        //     // Create a DrawingVisual to capture the visuals
        //     DrawingVisual drawingVisual = new DrawingVisual();
        //
        //     // Render the visuals into the DrawingVisual
        //     using (DrawingContext drawingContext = drawingVisual.RenderOpen())
        //     {
        //         // Render the current visuals into the DrawingVisual
        //         RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
        //             (int)ActualWidth, (int)ActualHeight, 98, 102, PixelFormats.Pbgra32);
        //
        //         renderTargetBitmap.Render(this); // Render the current visual tree to the bitmap
        //
        //         // Draw the bitmap into the DrawingVisual
        //         drawingContext.DrawImage(renderTargetBitmap, new Rect(0, 0, ActualWidth, ActualHeight));
        //     }
        //
        //     // Set the VisualBrush to the DrawingVisual
        //     visualBrush.Visual = drawingVisual;
        //
        //     // Set the Fill of the Rectangle to the VisualBrush
        //     blurOverlay.Fill = visualBrush;
        //
        //     // Create the BlurEffect
        //     blurOverlay.Effect = new BlurEffect
        //     {
        //         Radius = 20 // Set the blur radius
        //     };
        //
        //     // Add the Rectangle to the Grid
        //     ContainerGrid.Children.Add(blurOverlay);
        // }
        //
        // private void OpenSettingsWindow(object sender, RoutedEventArgs e)
        // {
        //     Console.WriteLine("Open Settings Window");
        //     isBlurToggled = !isBlurToggled;
        //     CreateBlurRectangle();
        //     Grid settingsContainerGrid = new Grid();
        //     settingsContainerGrid.Margin = new Thickness(0, 0, 0, 0);
        //
        //     ThicknessAnimation rollInAnimation = new ThicknessAnimation
        //     {
        //         From = new Thickness(0, 0, 0, -(int)ActualHeight),
        //         To = new Thickness(0, 0, 0, 0),
        //         Duration = new Duration(TimeSpan.FromSeconds(0.25)),
        //         FillBehavior = FillBehavior.HoldEnd, // Holds the end value after the animation completes
        //         EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        //     };
        //
        //     Rectangle settingsRect = new Rectangle
        //     {
        //         Width = (int)ActualWidth / 2,
        //         Height = (int)ActualHeight / 2,
        //         Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E2030")),
        //         RadiusX = Utils.SettingsRadius,
        //         RadiusY = Utils.SettingsRadius,
        //         Effect = Utils.dropShadowRectangle
        //     };
        //     settingsContainerGrid.Children.Add(settingsRect);
        //     Button closeButton = new Button
        //     {
        //         Style = (Style)Application.Current.FindResource("RoundedButton"),
        //         Content = "Close",
        //         Width = 80,
        //         Height = 30,
        //         VerticalAlignment = VerticalAlignment.Bottom,
        //         Margin = new Thickness(0, 0, 0, (int)ActualHeight * 0.25),
        //     };
        //     closeButton.Click += CloseSettingsWindow;
        //     settingsContainerGrid.Children.Add(closeButton);
        //
        //     TextBlock settingsTitle = new TextBlock
        //     {
        //         Text = "Settings",
        //         Foreground = new SolidColorBrush(Utils.FontColor),
        //         HorizontalAlignment = HorizontalAlignment.Center,
        //         VerticalAlignment = VerticalAlignment.Center,
        //         FontSize = 20,
        //         FontWeight = FontWeights.Bold,
        //     };
        //     settingsTitle.Margin = new Thickness(0, 0, 0, (int)(ActualHeight * 0.25) + 110);
        //     settingsContainerGrid.Children.Add(settingsTitle);
        //
        //     ContainerGrid.Children.Add(settingsContainerGrid);
        //     settingsContainerGrid.BeginAnimation(MarginProperty, rollInAnimation);
        // }
        //
        // private void CloseSettingsWindow(object sender, RoutedEventArgs e)
        // {
        //     // Create a list to store children that need to be removed
        //     List<UIElement> childrenToRemove = new List<UIElement>();
        //
        //     // Iterate through the children of the grid
        //     foreach (UIElement child in ContainerGrid.Children)
        //     {
        //         // Check if the child is a FrameworkElement and if its name does not match the one we want to keep
        //         if (child is FrameworkElement frameworkElement && frameworkElement.Name != "Grid")
        //         {
        //             childrenToRemove.Add(child); // Mark for removal
        //         }
        //     }
        //
        //     // Remove the marked children
        //     foreach (UIElement child in childrenToRemove)
        //     {
        //         ContainerGrid.Children.Remove(child);
        //     }
        // }

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