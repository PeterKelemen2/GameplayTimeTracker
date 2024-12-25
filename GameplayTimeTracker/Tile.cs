using System;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Application = System.Windows.Application;
using DateTime = System.DateTime;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Process = System.Diagnostics.Process;
using Task = System.Threading.Tasks.Task;


namespace GameplayTimeTracker;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public class Tile : UserControl
{
    private TileContainer _tileContainer;
    public Grid grid;
    public Rectangle container;

    public CustomButton EditButton { get; set; }
    private CustomButton RemoveButton { get; set; }
    private Button launchButton;
    private CustomButton LaunchButton { get; set; }
    private Image image;
    public Image bgImage;
    public Image bgImage2;
    public Grid iconContainerGrid;
    private TextBlock titleTextBlock;
    public TextBlock runningTextBlock;
    private TextBlock totalPlaytimeTitle;
    private TextBlock totalPlaytime;
    public TextBlock lastPlaytimeTitle;
    private TextBlock lastPlaytime;
    private TextBlock lastPlayDateTitleBlock;
    private TextBlock lastPlayDateBlock;

    private TextBlock sampleTextBlock;
    private TextBox sampleTextBox;

    public GradientBar totalTimeGradientBar;
    public GradientBar lastTimeGradientBar;
    public BitmapSource bgImageGray;
    public BitmapSource bgImageColor;
    public PopupMenu deleteMenu;
    private string absoluteIconPath;

    LinearGradientBrush gradientBrush;
    LinearGradientBrush editGradientBrush;

    private double currentPlaytime;
    private double bgColorOpacity = 0.8;
    private double bgGrayOpacity = 0.6;

    private const string? SampleImagePath = "assets/no_icon.png";
    private const string? Started = "Started:";
    private const string? Ended = "Ended:";

    public int Id { get; set; }
    public int Index { get; set; }
    public double TileWidth { get; set; }
    public double TileHeight { get; set; }
    public double CornerRadius { get; set; }
    public double TotalPlaytime { get; set; }
    public double TotalPlaytimePercent { get; set; }
    public double LastPlaytime { get; set; }
    public double LastPlaytimePercent { get; set; }
    public string LastPlayDateString { get; set; }
    public DateTime LastPlayDate { get; set; }
    public string? IconImagePath { get; set; }
    public string ExePath { get; set; }
    public string ShortcutArgs { get; set; }
    public string ExePathName { get; set; }
    public bool HorizontalTileG { get; set; }
    public bool HorizontalEditG { get; set; }
    public bool BigBgImages { get; set; }
    public bool IsRunning { get; set; }
    public bool WasRunning { get; set; }
    public bool WasMoved { get; set; }
    public EditMenu TileEditMenu { get; set; }
    public double TotalH { get; set; }
    public double TotalM { get; set; }
    public double TotalS { get; set; }
    public double LastH { get; set; }
    public double LastM { get; set; }
    public double LastS { get; set; }


    private double[] fColMarg;
    private double[] sColMarg;

    public void OpenExeFolder(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer.exe", $"/select,\"{ExePath}\"");
    }

    public void ToggleEdit_Click(object sender, RoutedEventArgs e)
    {
        if (TileEditMenu.IsOpen)
        {
            TileEditMenu.CloseMenu();
        }
        else
        {
            if (!grid.Children.Contains(TileEditMenu))
            {
                grid.Children.Add(TileEditMenu);
            }

            TileEditMenu.OpenMenu();
        }
    }

    public void editSaveButton_Click(object sender, RoutedEventArgs e)
    {
        SaveEditedData();

        TileEditMenu.ShowSaveIndicatorMethod();
    }

    public void SetLaunchButtonState()
    {
        if (!File.Exists(ExePath) || !System.IO.Path.GetExtension(ExePath).ToLower().Equals(".exe"))
        {
            LaunchButton.Disable();
        }
        else
        {
            LaunchButton.Enable();
        }
    }

    // Updates elements of the tile if there is change. Has a failsafe if new time values would cause a crash
    public void SaveEditedData()
    {
        bool toSave = false;
        // Save changed Name
        if (!GameName.Equals(TileEditMenu.TitleEditBox.Text))
        {
            string savedTitle = GameName;
            try
            {
                GameName = TileEditMenu.TitleEditBox.Text;
                titleTextBlock.Text = TileEditMenu.TitleEditBox.Text;
                toSave = true;
            }
            catch (Exception ex)
            {
                GameName = savedTitle;
                TileEditMenu.TitleEditBox.Text = savedTitle;
                titleTextBlock.Text = savedTitle;
                _tileContainer.InitSave();
                PopupMenu popupMenu =
                    new PopupMenu(text: "An error occured when saving new title!", type: PopupType.OK);
                popupMenu.OpenMenu();
            }
        }

        (double newH, double newM, double newS) =
            Utils.DecodeTimeString(TileEditMenu.PlaytimeEditBox.Text, TotalH, TotalM, TotalS);
        if (newH != TotalH || newM != TotalM || newS != TotalS)
        {
            (TotalH, TotalM, TotalS) = (newH, newM, newS + 1);
            TotalPlaytime = GetTotalPlaytimeAsDouble();
            TileEditMenu.PlaytimeEditBox.Text = Utils.GetPrettyTime(TotalPlaytime);
            _tileContainer.UpdatePlaytimeBars();
            UpdatePlaytimeText();
            Run TotalTimeBlockText = Utils.mainWindow.FindName("TotalTimeText") as Run;
            TotalTimeBlockText.Text = $"{Utils.GetPrettyTime(_tileContainer.GetTLTotalTimeDouble())}";
            toSave = true;
        }

        if (!ShortcutArgs.Equals(TileEditMenu.ArgsEditBox.Text))
        {
            ShortcutArgs = TileEditMenu.ArgsEditBox.Text;
            toSave = true;
        }

        // if (!ExePath.Equals(TileEditMenu.PathEditBox.Text))
        // {
        //     ExePath = TileEditMenu.PathEditBox.Text;
        //     SetLaunchButtonState();
        //     toSave = true;
        // }
        toSave = HandleNewExePath(TileEditMenu.PathEditBox.Text);

        if (toSave)
        {
            TileEditMenu.ToSave = true;
            _tileContainer.InitSave();
            Console.WriteLine("Data file saved!");
        }
    }

    // Sets new path for the exe chosen by the user
    public void UpdateExe(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        if (System.IO.Path.Exists(ExePath))
        {
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(ExePath);
        }

        openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;
            HandleNewExePath(filePath);
            // if (!ExePath.Equals(filePath))
            // {
            //     if (_tileContainer.GetTilesExePath().Contains(filePath))
            //     {
            //         PopupMenu popupMenu = new PopupMenu(textArray: new[]
            //             {
            //                 "This executable is already in use by",
            //                 $"{_tileContainer.GetTileNameByExePath(filePath)}",
            //                 "Would you like to select another executable?"
            //             },
            //             h: 220,
            //             textArrayFontSizes: new[] { 17, 20, 17 },
            //             type: PopupType.YesNo,
            //             yesClick: UpdateExe);
            //         popupMenu.OpenMenu();
            //     }
            //     else
            //     {
            //         ExePath = filePath;
            //         if (TileEditMenu.IsOpen) TileEditMenu.PathEditBox.Text = $"{ExePath}";
            //         SetLaunchButtonState();
            //         _tileContainer.InitSave();
            //     }
            // }
        }
    }

    private bool HandleNewExePath(string filePath)
    {
        if (!ExePath.Equals(filePath))
        {
            if (_tileContainer.GetTilesExePath().Contains(filePath))
            {
                PopupMenu popupMenu = new PopupMenu(textArray: new[]
                    {
                        "This executable is already in use by",
                        $"{_tileContainer.GetTileNameByExePath(filePath)}",
                        "Would you like to select another executable?"
                    },
                    h: 220,
                    textArrayFontSizes: new[] { 17, 20, 17 },
                    type: PopupType.YesNo,
                    yesClick: UpdateExe);
                popupMenu.OpenMenu();
                return false;
            }
            else
            {
                ExePath = filePath;
                if (TileEditMenu.IsOpen) TileEditMenu.PathEditBox.Text = $"{ExePath}";
                SetLaunchButtonState();
                _tileContainer.InitSave();
                return true;
            }
        }

        return false;
    }
    
    private async void LaunchExe(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IsRunning)
            {
                Console.WriteLine("Already running");
                return;
            }

            // Capture UI-bound properties on the UI thread
            string gameName = GameName; // Cache the value
            string exePath = ExePath; // Cache the value

            if (string.IsNullOrEmpty(exePath) || !System.IO.File.Exists(exePath))
            {
                throw new FileNotFoundException($"Executable not found: {exePath}");
            }

            string workingDir = System.IO.Path.GetDirectoryName(exePath);
            if (string.IsNullOrEmpty(workingDir) || !System.IO.Directory.Exists(workingDir))
            {
                throw new DirectoryNotFoundException($"Working directory not found: {workingDir}");
            }

            await Task.Run(() =>
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath, // Use the cached variable
                    Arguments = ShortcutArgs,
                    WorkingDirectory = workingDir,
                    UseShellExecute = true
                };

                Console.WriteLine($"Trying to launch {exePath} with arguments: {ShortcutArgs}");

                var process = Process.Start(startInfo);

                if (process != null)
                {
                    Console.WriteLine($"Launched {gameName}. Waiting for it to exit...");
                    process.WaitForExit();

                    // Notify the user on the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine($"{gameName} has exited. Exit code: {process.ExitCode}");
                    });
                }
                else
                {
                    // Notify the user on the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine($"Failed to start {gameName}");
                        PopupMenu popupMenu = new PopupMenu(text: $"Failed to start {gameName}", type: PopupType.OK);
                        popupMenu.OpenMenu();
                    });
                }
            });
        }
        catch (Win32Exception win32Ex) when (win32Ex.NativeErrorCode == 740) // Error 740 means elevation required
        {
            MessageBoxResult result = MessageBox.Show(
                $"The application {GameName} requires administrator privileges. Do you want to run it as administrator?",
                "Elevation Required", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = ExePath,
                    WorkingDirectory = System.IO.Path.GetDirectoryName(ExePath),
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(startInfo);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
            // Ensure error messages are shown on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                // MessageBox.Show($"Could not launch {GameName}\n\n{ex.Message}", "Something went wrong!",
                //     MessageBoxButton.OK, MessageBoxImage.Error);
                PopupMenu popupMenu = new PopupMenu(text: $"Failed to start {GameName}", type: PopupType.OK);
                popupMenu.OpenMenu();
            });
        }
    }

    private void OpenDeleteDialog(object sender, RoutedEventArgs e)
    {
        deleteMenu = new PopupMenu(text: $"Do you really want to delete {GameName}?", yesClick: DeleteTile);
        deleteMenu.OpenMenu();
        // DeleteTile(sender, e);
    }

    // Handles deletion of the instance
    public async void DeleteTile(object sender, RoutedEventArgs e)
    {
        // Delay for delete menu closing
        await Task.Delay(500);
        if (TileEditMenu != null)
        {
            if (TileEditMenu.IsOpen) TileEditMenu.CloseMenu();
        }

        double animationDuration = 0.7; // Duration for the animations
        double currentHeight = RenderSize.Height;

        if (RenderTransform is not ScaleTransform scaleTransform)
        {
            scaleTransform = new ScaleTransform(1, 1); // Initial scale (1 means no scaling)
            RenderTransform = scaleTransform;
            RenderTransformOrigin = new Point(0.0, 0.0); // Set origin to the top-left for scaling
        }

        // Create the Y scale animation for shrinking the tile
        DoubleAnimation scaleYAnimation = new DoubleAnimation
        {
            From = 1, // Original scale
            To = 0, // Shrink to zero
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration)),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        // Create the height animation for the tile's height
        DoubleAnimation heightAnimation = new DoubleAnimation
        {
            From = currentHeight * 2, // Start with the captured height
            To = 0, // Shrink to zero
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration)),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        Thickness targetMargin = new Thickness(-iconContainerGrid.Width / 2, -iconContainerGrid.Margin.Top, 0, 0);
        ThicknessAnimation marginAnimation = new ThicknessAnimation
        {
            From = iconContainerGrid.Margin, // Start from the current margin
            To = targetMargin, // Move out to the left
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration)),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        // Event handler for when the animations complete
        heightAnimation.Completed += (s, a) =>
        {
            // Remove the tile from the container and the parent after the animations
            _tileContainer.RemoveTileById(Id);
            Console.WriteLine("Updating bars from Tile - Delete");
            _tileContainer.UpdatePlaytimeBars();

            // If the tile's parent is a Panel, remove the tile from the panel's children
            if (Parent is Panel panel)
            {
                Console.WriteLine("Removing!!");
                panel.Children.Remove(this);
            }
        };

        // Apply the Y scale animation
        // scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);

        // Apply the height animation 
        container.Effect = null;
        BeginAnimation(MaxHeightProperty, heightAnimation);
        EditButton.BeginAnimation(OpacityProperty, scaleYAnimation);
        RemoveButton.BeginAnimation(OpacityProperty, scaleYAnimation);
        iconContainerGrid.BeginAnimation(MarginProperty, marginAnimation);
        iconContainerGrid.BeginAnimation(OpacityProperty, scaleYAnimation);
        // Optionally animate other properties like margin
        BeginAnimation(MarginProperty, Utils.GetMarginTopBottomAnimation(this, animationDuration));
    }

    public string GameName
    {
        get { return (string)GetValue(GameNameProperty); }
        set { SetValue(GameNameProperty, value); }
    }

    // Changes all the icon associated variables when updating icon
    public void UpdateIcons(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        if (System.IO.Path.Exists(ExePath))
        {
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(ExePath);
        }

        openFileDialog.Title = $"{GameName} - New Icon";
        openFileDialog.Filter =
            "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Executable files (*.exe)|*.exe|All files (*.*)|*.*";

        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;
            string fileName = System.IO.Path.GetFileName(filePath);
            fileName = fileName.Substring(0, fileName.Length - 4);

            string currentDirectory = Directory.GetCurrentDirectory();
            string potentialImagePath = System.IO.Path.Combine(Utils.SavedIconsPath, $"{fileName}.png");

            if (!Equals(filePath, potentialImagePath))
            {
                string uniqueFileName = $"{GameName}-{Guid.NewGuid().ToString()}.png";
                string? iconPath = System.IO.Path.Combine(Utils.SavedIconsPath, uniqueFileName);
                // string? iconPath = $"assets/{uniqueFileName}";

                Utils.PrepIcon(filePath, iconPath);
                iconPath = Utils.IsValidImage(iconPath) ? iconPath : SampleImagePath;
                IconImagePath = iconPath;
            }
            else
            {
                // IconImagePath = potentialImagePath;
                IconImagePath = System.IO.Path.Combine(Utils.SavedIconsPath, $"{fileName}.png");
            }

            UpdateImageVars();
        }
    }
    
    private BitmapImage LoadBitmapImage(string path)
    {
        using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            return bitmap;
        }
    }
    
    private void ClearImageReferences()
    {
        bgImageGray = null;
        bgImageColor = null;

        bgImage.Source = null;
        bgImage2.Source = null;
        image.Source = null;

        GC.Collect(); // Force garbage collection
        GC.WaitForPendingFinalizers();
    }

    private void SetupIconVars()
    {
        absoluteIconPath = System.IO.Path.GetFullPath(IconImagePath);

        if (!System.IO.File.Exists(absoluteIconPath))
        {
            // Handle the case where the file does not exist
            Console.WriteLine("Error: Icon file not found at " + absoluteIconPath);
            return;
        }

        // Load the images with OnLoad cache option
        var imageColor = new BitmapImage();
        imageColor.BeginInit();
        imageColor.UriSource = new Uri(absoluteIconPath, UriKind.Absolute);
        imageColor.CacheOption = BitmapCacheOption.OnLoad;
        imageColor.EndInit();

        bgImageColor = imageColor; // Color version
        bgImageGray = ConvertToGrayscale(imageColor); // Grayscale version
    }

    // TODO: Needs fixing, uses too much memory
    public BitmapSource ConvertToGrayscale(BitmapSource source)
    {
        var stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
        var pixels = new byte[stride * source.PixelWidth];

        source.CopyPixels(pixels, stride, 0);

        for (int i = 0; i < pixels.Length; i += 4)
        {
            // this works for PixelFormats.Bgra32
            var blue = pixels[i];
            var green = pixels[i + 1];
            var red = pixels[i + 2];
            var gray = (byte)(0.2126 * red + 0.7152 * green + 0.0722 * blue);
            pixels[i] = gray;
            pixels[i + 1] = gray;
            pixels[i + 2] = gray;
        }

        return BitmapSource.Create(
            source.PixelWidth, source.PixelHeight,
            source.DpiX, source.DpiY,
            source.Format, null, pixels, stride);
    }

    public void UpdateImageVars(bool toSave = true)
    {
        ClearImageReferences(); // Clear old images
        SetupIconVars(); // Load new images

        bgImage.Source = bgImageGray;
        bgImage2.Source = bgImageColor;
        image.Source = bgImageColor;

        ToggleBgImageColor(IsRunning);
        if (toSave) _tileContainer.InitSave();

        Console.WriteLine($"Icon for {GameName} changed to {absoluteIconPath}");
    }

    public void UpdateTileWidth(double newWidth)
    {
        TileWidth = newWidth;
        fColMarg = new[] { TileWidth * 0.25, TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin };
        sColMarg = new[] { TileWidth * 0.545, 1 };

        totalPlaytimeTitle.Margin = new Thickness(fColMarg[0], totalPlaytimeTitle.Margin.Top, 0, 0);
        totalPlaytime.Margin = new Thickness(fColMarg[0], totalPlaytime.Margin.Top, 0, 0);
        totalTimeGradientBar.Margin = new Thickness(fColMarg[0], totalTimeGradientBar.Margin.Top, 0, 0);
        totalTimeGradientBar.UpdateBarSizeWidth(TileWidth);

        lastPlaytime.Margin = new Thickness(sColMarg[0], lastPlaytime.Margin.Top, 0, 0);
        lastPlaytimeTitle.Margin = new Thickness(sColMarg[0], lastPlaytimeTitle.Margin.Top, 0, 0);
        lastTimeGradientBar.Margin = new Thickness(sColMarg[0], lastTimeGradientBar.Margin.Top, 0, 0);
        lastPlayDateTitleBlock.Margin = new Thickness(sColMarg[0], lastPlayDateTitleBlock.Margin.Top, 0, 0);
        lastPlayDateBlock.Margin = new Thickness(sColMarg[0] + 60, lastPlayDateBlock.Margin.Top, 0, 0);
        lastTimeGradientBar.UpdateBarSizeWidth(TileWidth);

        container.Width = TileWidth;
    }

    public void UpdatePlaytimeText()
    {
        totalPlaytime.Text = $"{TotalH}h {TotalM}m {TotalS}s";
        lastPlaytime.Text = $"{LastH}h {LastM}m {LastS}s";
        lastPlayDateTitleBlock.Text = Started;
        lastPlayDateBlock.Text = LastPlayDate.Year < 2000
            ? "Never"
            : $"{LastPlayDate.ToShortDateString().Replace(" ", "")} {LastPlayDate.ToShortTimeString()}";
    }

    public void UpdateDateInfo()
    {
        LastPlayDate = DateTime.Now;
        lastPlayDateTitleBlock.Text = Ended;
        lastPlayDateBlock.Text =
            $"{LastPlayDate.ToShortDateString().Replace(" ", "")} {LastPlayDate.ToShortTimeString()}";
    }

    public void IncrementPlaytime()
    {
        double min = 60 - 1;
        LastS++;
        if (LastS > min)
        {
            LastS = 0;
            LastM++;
            if (LastM > min)
            {
                LastH++;
                LastM = 0;
            }
        }

        TotalS++;
        if (TotalS > min)
        {
            TotalS = 0;
            TotalM++;
            if (TotalM > min)
            {
                TotalH++;
                TotalM = 0;
            }
        }

        // TotalPlaytime = GetTotalPlaytimeAsDouble();
        // LastPlaytime = GetLastPlaytimeAsDouble();
    }

    public double GetTotalPlaytimeAsDouble()
    {
        return TotalH + (TotalM / 60) + (TotalS / 3600);
    }

    public double GetLastPlaytimeAsDouble()
    {
        return LastH + (LastM / 60) + (LastS / 3600);
    }

    // Resets all the values associated with an ongoing playtime calculation.
    public void ResetLastPlaytime()
    {
        LastH = 0;
        LastM = 0;
        LastS = 0;
        LastPlaytime = 0;
        LastPlaytimePercent = 0;
        LastPlayDate = DateTime.Now;
        UpdatePlaytimeText();
        _tileContainer.UpdateLastPlaytimeBarOfTile(Id);
        _tileContainer.InitSave();
    }

    public static readonly DependencyProperty GameNameProperty =
        DependencyProperty.Register("GameName", typeof(string), typeof(Tile), new PropertyMetadata(""));

    public Tile()
    {
    }

    public Tile(TileContainer tileContainer, string gameName, DateTime lastPlayDate, bool horizontalTile = true,
        bool horizontalEdit = true, bool bigBgImages = false, double totalTime = 0, double lastPlayedTime = 0,
        string? iconImagePath = SampleImagePath, string exePath = "", string shortcutArgs = "", double width = 760)
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
        WasRunning = false;
        IsRunning = false;

        _tileContainer = tileContainer;
        GameName = gameName;
        TileWidth = width;
        TileHeight = Utils.THeight;
        CornerRadius = Utils.BorderRadius;

        Console.WriteLine(lastPlayDate.Year);
        if (lastPlayDate.Year < 2000)
        {
            LastPlayDateString = "Never";
        }
        else
        {
            LastPlayDate = lastPlayDate;
            LastPlayDateString =
                $"{lastPlayDate.ToShortDateString().Replace(" ", "")} {lastPlayDate.ToShortTimeString()}";
        }

        TotalPlaytime = totalTime < 0 ? 0 : totalTime;
        LastPlaytime = lastPlayedTime > TotalPlaytime ? TotalPlaytime : (lastPlayedTime < 0 ? 0 : lastPlayedTime);
        TotalPlaytimePercent = TotalPlaytime / tileContainer.GetTLTotalTimeDouble();
        LastPlaytimePercent = LastPlaytime / TotalPlaytime;

        (TotalH, TotalM, TotalS) = Utils.ConvertDoubleToTime(TotalPlaytime);
        (LastH, LastM, LastS) = Utils.ConvertDoubleToTime(LastPlaytime);

        IconImagePath = iconImagePath == null ? SampleImagePath : iconImagePath;
        if (!File.Exists(IconImagePath)) IconImagePath = SampleImagePath;

        ExePath = exePath;
        ExePathName = System.IO.Path.GetFileNameWithoutExtension(ExePath);

        ShortcutArgs = shortcutArgs.Length > 0 ? shortcutArgs : "";

        HorizontalTileG = horizontalTile;
        HorizontalEditG = horizontalEdit;
        BigBgImages = bigBgImages;

        WasMoved = false;
        SetupIconVars();
    }

    public void UpdateTileColors()
    {
        SetGradients();
        container.Fill = gradientBrush;

        titleTextBlock.Foreground = new SolidColorBrush(Utils.FontColor);
        runningTextBlock.Foreground = new SolidColorBrush(Utils.RunningColor);

        totalPlaytimeTitle.Foreground = new SolidColorBrush(Utils.FontColor);
        totalPlaytime.Foreground = new SolidColorBrush(Utils.FontColor);
        totalTimeGradientBar.barForeground.Fill = Utils.createLinGradBrushHor(Utils.LeftColor, Utils.RightColor);

        lastPlaytimeTitle.Foreground = new SolidColorBrush(Utils.FontColor);
        lastPlaytime.Foreground = new SolidColorBrush(Utils.FontColor);
        lastPlayDateBlock.Foreground = new SolidColorBrush(Utils.FontColor);
        lastPlayDateTitleBlock.Foreground = new SolidColorBrush(Utils.FontColor);
        lastTimeGradientBar.barForeground.Fill = Utils.createLinGradBrushHor(Utils.LeftColor, Utils.RightColor);

        runningTextBlock.Foreground = new SolidColorBrush(Utils.RunningColor);
    }

    public void SetGradients()
    {
        gradientBrush = HorizontalTileG
            ? Utils.createLinGradBrushHor(Utils.TileColor1, Utils.TileColor2)
            : Utils.createLinGradBrushVer(Utils.TileColor1, Utils.TileColor2);

        editGradientBrush = HorizontalEditG
            ? Utils.createLinGradBrushHor(Utils.EditColor1, Utils.EditColor2)
            : Utils.createLinGradBrushVer(Utils.EditColor1, Utils.EditColor2);

        if (container != null) container.Fill = gradientBrush;

        if (TileEditMenu != null && TileEditMenu.IsOpen)
        {
            TileEditMenu.BgRectangle.Fill = editGradientBrush;
        }
    }

    // Sets up elements of the tile with the default values
    public void InitializeTile()
    {
        // LastPlayDate = DateTime.Now;
        // LastPlayDateString = $"{LastPlayDate.ToShortDateString()} {LastPlayDate.ToShortTimeString()}";

        fColMarg = new[] { TileWidth * 0.25, TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin };
        sColMarg = new[] { TileWidth * 0.545, 1 };

        SetGradients();

        sampleTextBlock = Utils.NewTextBlock();

        sampleTextBox = Utils.NewTextBoxEdit();
        sampleTextBox.Style = (Style)Application.Current.FindResource("RoundedTextBox");

        // Create a Grid to hold the Rectangle and TextBlock
        grid = new Grid();

        // Define the grid rows
        RowDefinition row1 = new RowDefinition();
        RowDefinition row2 = new RowDefinition();
        grid.RowDefinitions.Add(row1);
        grid.RowDefinitions.Add(row2);

        // Create the second Rectangle
        container = new Rectangle
        {
            Width = TileWidth,
            Height = TileHeight,
            RadiusX = CornerRadius,
            RadiusY = CornerRadius,
            Fill = gradientBrush,
            Margin = new Thickness(Utils.TileLeftMargin, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Effect = Utils.dropShadowIcon
        };
        int topMargin = -40;

        EditButton = new CustomButton(width: 40, height: 40, buttonImagePath: Utils.EditIcon,
            type: ButtonType.Default);
        EditButton.HorizontalAlignment = HorizontalAlignment.Right;
        EditButton.VerticalAlignment = VerticalAlignment.Center;
        EditButton.Margin = new Thickness(0, topMargin, 100, 0);
        EditButton.Click += ToggleEdit_Click;
        Panel.SetZIndex(EditButton, 3);
        grid.Children.Add(EditButton);

        RemoveButton = new CustomButton(width: 40, height: 40, buttonImagePath: Utils.RemIcon,
            type: ButtonType.Negative);
        RemoveButton.HorizontalAlignment = HorizontalAlignment.Right;
        RemoveButton.VerticalAlignment = VerticalAlignment.Center;
        RemoveButton.Margin = new Thickness(0, topMargin, 50, 0);
        RemoveButton.Click += OpenDeleteDialog;
        Panel.SetZIndex(RemoveButton, 3);
        grid.Children.Add(RemoveButton);

        LaunchButton = new CustomButton(text: "Launch", width: 90, height: 40,
            type: ButtonType.Positive);
        LaunchButton.HorizontalAlignment = HorizontalAlignment.Right;
        LaunchButton.VerticalAlignment = VerticalAlignment.Center;
        LaunchButton.Margin = new Thickness(0, 60, 50, 0);
        SetLaunchButtonState();
        Panel.SetZIndex(LaunchButton, 3);
        grid.Children.Add(LaunchButton);
        LaunchButton.Click += LaunchExe;

        Grid.SetRow(container, 0);
        Grid.SetRow(RemoveButton, 0);

        Panel.SetZIndex(container, 1);

        grid.Children.Add(container);

        titleTextBlock = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        titleTextBlock.Text = GameName;
        titleTextBlock.FontSize = Utils.TitleFontSize;
        titleTextBlock.Margin = new Thickness(Utils.TextMargin * 2, Utils.TextMargin / 2, 0, 0);
        TextOptions.SetTextRenderingMode(titleTextBlock, TextRenderingMode.ClearType);
        TextOptions.SetTextFormattingMode(titleTextBlock, TextFormattingMode.Ideal);

        runningTextBlock = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        runningTextBlock.Text = "Running!";
        runningTextBlock.FontSize = Utils.TitleFontSize - 4;
        runningTextBlock.Foreground = new SolidColorBrush(Utils.RunningColor);
        runningTextBlock.Margin =
            new Thickness(Utils.TextMargin * 2, Utils.TextMargin / 2 + Utils.TitleFontSize + 3, 0, 0);

        // Add the TextBlock to the grid
        Grid.SetRow(titleTextBlock, 0);
        Grid.SetRow(runningTextBlock, 0);
        grid.Children.Add(titleTextBlock);
        grid.Children.Add(runningTextBlock);
        Panel.SetZIndex(titleTextBlock, 3);
        Panel.SetZIndex(runningTextBlock, 3);

        if (!IsRunning) runningTextBlock.Text = "";

        // Create the Image and other UI elements, positioning them in the second row as well
        if (IconImagePath != null)
        {
            iconContainerGrid = new Grid();
            iconContainerGrid = GetBgImagesInGrid(BigBgImages);
        }
        else
        {
            Console.WriteLine("Icon was null");
        }

        // Add all other elements as before, positioning them in the second row
        Grid.SetRow(iconContainerGrid, 0);
        grid.Children.Add(iconContainerGrid);
        // Add playtime elements

        // Top margin modifier
        int[] tmm = { -17, 5, 30, 65 };

        totalPlaytimeTitle = Utils.CloneTextBlock(sampleTextBlock);
        totalPlaytimeTitle.Text = "Total Playtime:";
        totalPlaytimeTitle.Margin =
            new Thickness(fColMarg[0], fColMarg[1] + tmm[0], 0, 0);
        // new Thickness(fColMarg[0], fColMarg[1] - 10, 0, 0);

        totalPlaytime = Utils.CloneTextBlock(sampleTextBlock, isBold: false);
        totalPlaytime.Text = $"{TotalH}h {TotalM}m {TotalS}s";
        totalPlaytime.Margin = Margin =
            new Thickness(fColMarg[0], fColMarg[1] + tmm[1], 0, 0);

        SetPlaytimeBars(tmm[2]);

        lastPlaytimeTitle = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        lastPlaytimeTitle.Text = "Last Session:";
        lastPlaytimeTitle.Margin = new Thickness(sColMarg[0], fColMarg[1] + tmm[0], 0, 0);

        lastPlaytime = Utils.CloneTextBlock(sampleTextBlock, isBold: false);
        lastPlaytime.Text = $"{LastH}h {LastM}m {LastS}s";
        lastPlaytime.Margin = new Thickness(sColMarg[0], fColMarg[1] + tmm[1], 0, 0);

        lastPlayDateTitleBlock = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        lastPlayDateTitleBlock.Text = LastPlayDate.Year > 1999 ? Ended : Started;
        lastPlayDateTitleBlock.Margin = new Thickness(sColMarg[0], fColMarg[1] + tmm[3], 0, 0);

        lastPlayDateBlock = Utils.CloneTextBlock(sampleTextBlock, isBold: false);
        lastPlayDateBlock.Text = LastPlayDateString;
        lastPlayDateBlock.Margin = new Thickness(sColMarg[0] + 60, fColMarg[1] + tmm[3], 0, 0);

        Panel.SetZIndex(totalPlaytimeTitle, 3);
        Panel.SetZIndex(totalPlaytime, 3);
        Panel.SetZIndex(totalTimeGradientBar, 3);

        Grid.SetRow(totalPlaytimeTitle, 0);
        Grid.SetRow(totalPlaytime, 0);
        Grid.SetRow(totalTimeGradientBar, 0);

        grid.Children.Add(totalPlaytimeTitle);
        grid.Children.Add(totalPlaytime);
        grid.Children.Add(totalTimeGradientBar);
        grid.Children.Add(lastPlaytimeTitle);
        grid.Children.Add(lastPlaytime);
        grid.Children.Add(lastTimeGradientBar);
        grid.Children.Add(lastPlayDateTitleBlock);
        grid.Children.Add(lastPlayDateBlock);

        Panel.SetZIndex(lastPlaytimeTitle, 3);
        Panel.SetZIndex(lastPlaytime, 3);
        Panel.SetZIndex(lastTimeGradientBar, 3);
        Panel.SetZIndex(lastPlayDateTitleBlock, 3);
        Panel.SetZIndex(lastPlayDateBlock, 3);

        Panel.SetZIndex(iconContainerGrid, 2);

        TileEditMenu = new EditMenu(this);
        Grid.SetRow(TileEditMenu, 1);
        TileEditMenu.IsOpen = false;
        // grid.Children.Add(TileEditMenu);

        // Set the Grid as the content of the UserControl
        Content = grid;
    }

    public Grid GetBgImagesInGrid(bool bigImages)
    {
        var localIconContainerGrid = new Grid
        {
            Width = TileWidth,
            Height = TileHeight,
            ClipToBounds = false,
            Margin = new Thickness(0, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
        };

        image = new Image
        {
            Source = new BitmapImage(new Uri(absoluteIconPath, UriKind.Absolute)),
            Stretch = Stretch.UniformToFill,
            Width = TileHeight / 2,
            Height = TileHeight / 2,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 20, 0, 0),
        };

        bgImage = new Image
        {
            Source = bgImageGray,
            Stretch = Stretch.UniformToFill,
            Width = bigImages ? TileWidth / 2 - 20 : TileWidth / 2 - 140,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Opacity = bgGrayOpacity
        };

        bgImage2 = new Image
        {
            Source = bgImageColor,
            Stretch = bgImage.Stretch,
            Width = bgImage.Width,
            Height = bgImage.Height,
            HorizontalAlignment = bgImage.HorizontalAlignment,
            VerticalAlignment = bgImage.VerticalAlignment,
            Opacity = 0.0
        };

        var imageBorder = new Border
        {
            Padding = new Thickness(Utils.dropShadowIcon.BlurRadius),
            Child = image,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(60, 0, 0, 0),
            Effect = Utils.dropShadowIcon,
        };

        var bgImageBorder = new Border
        {
            Padding = new Thickness(Utils.blurEffect.Radius * 3, 0, Utils.blurEffect.Radius, 0),
            Child = bgImage,
            Height = localIconContainerGrid.Height + Utils.blurEffect.Radius * 2,
            Width = iconContainerGrid.Width,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Effect = Utils.blurEffect
        };

        var bgImageBorder2 = new Border
        {
            Padding = bgImageBorder.Padding,
            Child = bgImage2,
            Height = bgImageBorder.Height,
            Width = bgImageBorder.Width,
            HorizontalAlignment = bgImageBorder.HorizontalAlignment,
            VerticalAlignment = bgImageBorder.VerticalAlignment,
            Effect = bgImageBorder.Effect
        };

        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
        RenderOptions.SetBitmapScalingMode(bgImage, BitmapScalingMode.HighQuality);
        RenderOptions.SetBitmapScalingMode(bgImage2, BitmapScalingMode.HighQuality);
        localIconContainerGrid.Children.Add(bgImageBorder);
        localIconContainerGrid.Children.Add(bgImageBorder2);
        localIconContainerGrid.Children.Add(imageBorder);

        return localIconContainerGrid;
    }

    // Creates the custom playtime bars
    public void SetPlaytimeBars(int tm)
    {
        totalTimeGradientBar = new GradientBar(this, percent: TotalPlaytimePercent)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin =
                new Thickness(fColMarg[0] - 3, fColMarg[1] + tm, 0, 0),
            // TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin + 40, 0, 0),
            Effect = Utils.dropShadowText,
        };

        lastTimeGradientBar = new GradientBar(this, percent: LastPlaytimePercent)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness((Utils.TextMargin + TileHeight + 20) * 2.3, fColMarg[1] + tm, 0, 0),
            Effect = Utils.dropShadowText,
        };
    }

    // Toggles between the two states oif the background image. Color for running, gray for not running
    public void ToggleBgImageColor(bool runningBool)
    {
        DoubleAnimation fadeInColored = new DoubleAnimation
        {
            From = 0.0,
            To = bgColorOpacity,
            Duration = TimeSpan.FromSeconds(0.75),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        DoubleAnimation fadeOutColored = new DoubleAnimation
        {
            From = bgColorOpacity,
            To = 0.0,
            Duration = TimeSpan.FromSeconds(0.75),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        DoubleAnimation fadeInGray = new DoubleAnimation
        {
            From = 0.0,
            To = bgGrayOpacity,
            Duration = TimeSpan.FromSeconds(0.75),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        DoubleAnimation fadeOutGray = new DoubleAnimation
        {
            From = bgGrayOpacity,
            To = 0.0,
            Duration = TimeSpan.FromSeconds(0.75),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        if (runningBool)
        {
            // bgImage - Gray
            // bgImage2 - Color

            if (bgImage.Opacity > 0.0) // Gray
            {
                bgImage.BeginAnimation(UIElement.OpacityProperty, fadeOutGray);
                bgImage2.BeginAnimation(UIElement.OpacityProperty, fadeInColored);
            }
        }
        else
        {
            if (bgImage2.Opacity > 0.0) // Color
            {
                bgImage.BeginAnimation(UIElement.OpacityProperty, fadeInGray);
                bgImage2.BeginAnimation(UIElement.OpacityProperty, fadeOutColored);
            }
        }
    }

    public void SetBgImageSizes(bool value)
    {
        grid.Children.Remove(iconContainerGrid);
        iconContainerGrid.Children.Clear();
        iconContainerGrid.Children.Add(GetBgImagesInGrid(value));
        Panel.SetZIndex(iconContainerGrid, 2);
        grid.Children.Add(iconContainerGrid);
        ToggleBgImageColor(IsRunning);
    }
}