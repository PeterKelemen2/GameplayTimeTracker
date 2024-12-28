using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Shellify;
using Action = System.Action;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Grid = System.Windows.Controls.Grid;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;

namespace GameplayTimeTracker
{
    public partial class MainWindow : System.Windows.Window
    {
        private const string? SampleImagePath = "assets/no_icon.png";
        private bool isBlurToggled = false;
        private bool isAnimating = false;

        TileContainer tileContainer = new();

        public JsonHandler handler = new();
        ProcessTracker tracker = new();

        List<Theme> themesList = new List<Theme>();
        private Settings settings;

        private NotificationHandler notificationHandler = new();

        private PopupMenu exitPopup;
        private PopupMenu selfPopup;
        private SettingsMenu settingsMenu;
        private DragDropOverlay dragDropOverlay;
        private CustomButton SettingsButton;
        private CustomButton AddButton;

        public EntryRepository entryRepository;


        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            TotalTimeText.Text = Utils.GetPrettyTime(tileContainer.GetTLTotalTimeDouble());
            tileContainer.TotalTimeRun = TotalTimeText;
            tracker.InitializeProcessTracker(tileContainer, entryRepository);

            // tracker.entryRepository = entryRepository;

            UpdateStackPane();
            GameCountRun.Text = $"{tileContainer.tilesList.Count}";
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
                    theme.Colors = Utils.CheckThemeIntegrity(theme.Colors);
                    if (theme.ThemeName.Equals(themeName))
                    {
                        try
                        {
                            SolidColorBrush scb = new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString(theme.Colors["bgColor"]));
                            ScrollViewer.Background = scb;
                            MainStackPanel.Background = scb;
                            Grid.Background = scb;

                            Grid gridFooter = (Grid)FindName("Footer");
                            gridFooter.Background = new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString(theme.Colors["footerColor"]));

                            Utils.SetColors(theme.Colors);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Failed to load theme " + theme.ThemeName);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No theme found, adding default");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            entryRepository = new EntryRepository();

            notificationHandler = new NotificationHandler();
            InitSettings();
            handler.InitializeContainer(tileContainer, entryRepository, settings);
            dragDropOverlay = new DragDropOverlay();
            DragDropGrid.Children.Add(dragDropOverlay);
            // CheckToUpdate();    

            Closing += MainWindow_Closing;
            Loaded += OnLoaded;
            ContentRendered += MainWindow_ContentRendered;
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            CreateButtonsOnFooter();
            ShowTilesOnCanvas();
            CheckToUpdate();
        }

        private void CreateButtonsOnFooter()
        {
            AddButton = new CustomButton(width: 40, height: 40, hA: HorizontalAlignment.Left,
                buttonImagePath: Utils.AddIcon);
            AddButton.Margin = new Thickness(15, 0, 0, 0);
            AddButton.Click += AddExecButton_Click;
            Grid.SetRow(AddButton, 1);
            Grid.Children.Add(AddButton);

            SettingsButton = new CustomButton(width: 40, height: 40, hA: HorizontalAlignment.Left,
                buttonImagePath: Utils.CogIcon);
            SettingsButton.Margin = new Thickness(70, 0, 0, 0);
            SettingsButton.Click += OpenSettingsWindow;
            Grid.SetRow(SettingsButton, 1);
            Grid.Children.Add(SettingsButton);
        }

        private void CheckToUpdate()
        {
            if (handler.CheckForDataToUpdate() && settings.DataNeedsUpdating)
            {
                if (tileContainer.tilesList.Count > 0)
                {
                    PopupMenu popupMenu =
                        new PopupMenu(
                            textArray: new[]
                            {
                                "It seems like your data needs updating!",
                                "Would you like to update it now?",
                                "Don't worry, your current data will be backed up!"
                            },
                            textArrayFontSizes: new[] { 20, 20, 20 },
                            h: 260, type: PopupType.YesNo, yesClick: ToUpdate_Click);
                    popupMenu.OpenMenu();
                }
                else
                {
                    settings.DataNeedsUpdating = false;
                    handler.WriteSettingsToFile(settings);
                }
            }
        }

        public void ToUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
        }

        public void UpdateData()
        {
            handler.BackupDataFile();
            tileContainer.UpdateLegacyTime();
            settings.DataNeedsUpdating = false;
            handler.WriteSettingsToFile(settings);
        }

        private void UpdateColors()
        {
            InitSettings();
            tileContainer.UpdateTilesColors();
            ShowTilesOnCanvas();
            AddButton.SetButtonColors();
            SettingsButton.SetButtonColors();
            // TotalPlaytimeTextBlock.Foreground = new SolidColorBrush(Utils.FontColor);
            // GamesLoadedBlock.Foreground = new SolidColorBrush(Utils.FontColor);
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
                        tracker.HandleProcesses();
                        tracker.HandleProcessesNew();
                        RearrangeTiles();
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
            string arguments = "";
            string exePath = "";
            if (Path.GetExtension(filePath).Equals(".lnk", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Selected file is a shortcut!");
                var shortcut = ShellLinkFile.Load(filePath);
                Console.WriteLine("Loaded shortcut!");

                exePath += shortcut.LinkInfo.LocalBasePath;
                Console.WriteLine($"Exe path: {shortcut.LinkInfo.LocalBasePath}");

                arguments += shortcut.Arguments;
                Console.WriteLine($"Arguments: {shortcut.Arguments}");
            }
            else if (Path.GetExtension(filePath).Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                exePath += filePath;
            }

            if (tileContainer.IsExePathPresent(exePath))
            {
                PopupMenu popupMenu = new(text: $"{exePath} is already on the list",
                    type: PopupType.OK);
                popupMenu.OpenMenu();
            }
            else
            {
                Console.WriteLine($"Creating icon for {exePath}");
                string fileName = Path.GetFileName(exePath);
                fileName = fileName.Substring(0, fileName.Length - 4);

                string uniqueFileName = $"{fileName}-{Guid.NewGuid().ToString()}.png";
                string iconPath = Path.Combine(Utils.SavedIconsPath, uniqueFileName);

                Utils.PrepIcon(exePath, iconPath);
                iconPath = Utils.IsValidImage(iconPath) ? iconPath : SampleImagePath;

                Entry newEntry = new Entry();
                newEntry.Name = fileName;
                newEntry.ExePath = exePath;
                newEntry.IconPath = iconPath;
                newEntry.Arguments = arguments;
                entryRepository.AddEntry(newEntry);

                try
                {
                    Console.WriteLine($"Trying to add: {fileName}");
                    // Tile newTile = new Tile(tileContainer, fileName, new DateTime(1999, 1, 1, 1, 1, 1),
                    //     settings.HorizontalTileGradient,
                    //     settings.HorizontalEditGradient, settings.BigBgImages, iconImagePath: iconPath,
                    //     exePath: exePath,
                    //     shortcutArgs: arguments.Length > 0 ? arguments : "",
                    //     data: newEntry
                    // );
                    Tile newTile = new Tile(tileContainer, newEntry, settings);

                    newTile.Margin = new Thickness(Utils.TileLeftMargin, 5, 0, 5);

                    if (!(Path.GetFileName(filePath).Equals("GameplayTimeTracker.exe") ||
                          Path.GetFileName(filePath).Equals("Gameplay Time Tracker.exe")))
                    {
                        tileContainer.AddTile(newTile, newlyAdded: true);
                        // tileContainer.ListTiles();
                        ShowTilesOnCanvas();
                        // handler.WriteContentToFile(tileContainer, Utils.DataFilePath);
                        handler.WriteEntriesToFile(entryRepository);
                    }
                    else
                    {
                        selfPopup = new PopupMenu(text: "Sorry, can't keep tabs on myself", type: PopupType.OK);
                        selfPopup.OpenMenu();
                    }
                }
                catch (Exception e)
                {
                    PopupMenu popupMenu = new(text: $"Something went wrong adding {filePath}", type: PopupType.OK);
                    popupMenu.OpenMenu();
                    Console.WriteLine(e);
                }
            }
        }


        private void AddExecButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe, *.lnk)|*.exe;*.lnk|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;
                Console.WriteLine($"Selected File: {selectedFile}");
                try
                {
                    AddEntry(selectedFile);
                }
                catch (Exception exception)
                {
                    PopupMenu popupMenu = new(text: $"Something went wrong adding {selectedFile}", type: PopupType.OK);
                    popupMenu.OpenMenu();
                    Console.WriteLine(exception);
                }
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

        public void ShowTilesOnCanvas()
        {
            MainStackPanel.Children.Clear();
            foreach (var tile in tileContainer.tilesList)
            {
                double lm = (Width - tile.TileWidth) * 0.5 + ScrollViewer.Padding.Left - 1;
                tile.Margin = new Thickness(lm, 5, 0, 5);

                MainStackPanel.Children.Add(tile);
            }
        }

        public void ShowScrollViewerOverlay(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            OverlayTop.Visibility = e.VerticalOffset > 0 ? Visibility.Visible : Visibility.Collapsed;
            OverlayBottom.Visibility = e.VerticalOffset < ScrollViewer.ScrollableHeight
                ? Visibility.Visible
                : Visibility.Collapsed;
            OverlayTop.Width = scrollViewer.ViewportWidth * 2;
            OverlayBottom.Width = scrollViewer.ViewportWidth * 2;
        }

        private void CloseSettingsMenu()
        {
            if (settingsMenu != null)
            {
                if (settingsMenu.IsToggled)
                {
                    settingsMenu.CloseMenuMethod();
                }
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;

                tileContainer.CloseAllPopups();
                CloseSettingsMenu();
                if (exitPopup == null || !exitPopup.IsToggled)
                {
                    exitPopup = new PopupMenu(text: "Are you sure you want to exit?",
                        yesClick: ExitButton_Click, noClick: ExitButton_NoClick);
                    exitPopup.OpenMenu();
                }

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

        private void ExitButton_NoClick(object sender, RoutedEventArgs e)
        {
            exitPopup.IsToggled = false;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // tileContainer?.InitSave();
            handler.WriteEntriesToFile(entryRepository);
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
            settingsMenu = new SettingsMenu(containerGrid: ContainerGrid,
                menuGrid: SettingsGrid,
                settings: handler.GetSettingsFromFile(),
                tileContainer,
                updateMethod: UpdateColors,
                tileGradMethod: tileContainer.UpdateTilesGradients,
                tileBgImagesMethod: tileContainer.UpdateTileBgImages,
                showTilesOnCanvasMethod: ShowTilesOnCanvas
                // updateLegacyMethod: tileContainer.UpdateLegacyTime
            );
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

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newWidth = ActualWidth - 2 * Utils.TileLeftMargin - 1.5 * SystemParameters.VerticalScrollBarWidth;
            Console.WriteLine(ActualWidth);
            tileContainer.UpdateTilesWidth(newWidth);
        }
    }
}