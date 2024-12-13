using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Shellify;
using ShellLink.Structures;
using WindowsShortcutFactory;
using Application = System.Windows.Application;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Key = System.Windows.Input.Key;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;


namespace GameplayTimeTracker;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public class Tile : UserControl
{
    private TileContainer _tileContainer;

    public bool isMenuOpen = false;

    public bool isRunning = false;
    private bool isRunningGame = false;
    public bool wasRunning = false;

    public Grid grid;
    private Rectangle menuRectangle;
    private Rectangle shadowRectangle;
    public Rectangle container;
    private Button editButton;
    private Button removeButton;
    private Button editSaveButton;
    private Button changeIconButton;
    private Button launchButton;
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
    private TextBlock sampleTextBlock;
    private TextBox editNameBox;
    private TextBox sampleTextBox;

    private TextBlock editNameTitle;
    private TextBox editPlaytimeBox;
    private TextBlock editPlaytimeTitle;

    private TextBlock editExePathTitle;
    private TextBox editExePathBox;
    private Button editExePathButton;

    public GradientBar totalTimeGradientBar;
    public GradientBar lastTimeGradientBar;
    public BitmapSource bgImageGray;
    public BitmapSource bgImageColor;
    public PopupMenu deleteMenu;
    private string absoluteIconPath;

    LinearGradientBrush gradientBrush;
    LinearGradientBrush editGradientBrush;

    List<UIElement> editElements = new List<UIElement>();
    List<UIElement> mainElements = new List<UIElement>();
    List<UIElement> animatedElements = new List<UIElement>();

    public double hTotal;
    public double mTotal;
    private double hLast;
    private double mLast;
    private double currentPlaytime;
    private double bgColorOpacity = 0.8;
    private double bgGrayOpacity = 0.6;

    private const string? SampleImagePath = "assets/no_icon.png";

    public int Id { get; set; }
    public int Index { get; set; }
    public double TileWidth { get; set; }
    public double TileHeight { get; set; }
    public double CornerRadius { get; set; }

    public double TotalPlaytime { get; set; }
    public double TotalPlaytimePercent { get; set; }
    public double LastPlaytime { get; set; }
    public double LastPlaytimePercent { get; set; }
    public string? IconImagePath { get; set; }
    public string ExePath { get; set; }
    public string ShortcutArgs { get; set; }
    public string ExePathName { get; set; }

    public bool HorizontalTileG { get; set; }
    public bool HorizontalEditG { get; set; }
    public bool BigBgImages { get; set; }

    public double CurrentPlaytime { get; set; }
    public double HTotal { get; set; }
    public double HLast { get; set; }
    public double MTotal { get; set; }
    public double MLast { get; set; }
    public bool IsRunning { get; set; }
    public bool IsMenuToggled { get; set; }

    public bool IsRunningGame { get; set; }
    public bool WasRunning { get; set; }
    public bool WasOpened { get; set; }
    public bool WasMoved { get; set; }

    public EditMenu TileEditMenu { get; set; }

    private double[] fColMarg;
    private double[] sColMarg;


    private Dictionary<UIElement, Thickness> originalMargins = new();
    private Dictionary<UIElement, double> originalHeights = new();
    // private bool wasOnceOpened = false;

    public void OpenExeFolder(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer.exe", $"/select,\"{ExePath}\"");
    }

    // Sets the margin of the edit menu elements and plays an animation to bring them down from under the tile
    public void ToggleEdit()
    {
        isMenuOpen = !isMenuOpen;
        IsMenuToggled = !IsMenuToggled;
        isMenuOpen = IsMenuToggled;
        double animationDuration = 0.35;

        DoubleAnimation heightAnimation = new DoubleAnimation
        {
            From = isMenuOpen ? 0 : TileHeight + 20, To = isMenuOpen ? TileHeight + 20 : 0,
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration)),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        DoubleAnimation opacityAnimation = new DoubleAnimation
        {
            From = isMenuOpen ? 0 : 1, To = isMenuOpen ? 1 : 0,
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration)),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        if (!WasOpened)
        {
            double padding = 3;
            // menuRectangle.MaxHeight = TileHeight + 40;
            // shadowRectangle.MaxHeight = 30;

            editNameTitle.MaxHeight = Utils.EditTextMaxHeight;
            editNameBox.Height = Utils.TextBoxHeight;
            editNameBox.MaxHeight = Utils.TextBoxHeight;

            editPlaytimeBox.MaxHeight = Utils.EditTextMaxHeight;
            editPlaytimeTitle.MaxHeight = Utils.EditTextMaxHeight;

            // changeIconButton.MaxHeight = Utils.EditTextMaxHeight;
            editSaveButton.Height = Utils.TextBoxHeight;
            editSaveButton.Width = changeIconButton.Width;
            // editSaveButton.MaxHeight = Utils.TextBoxHeight;

            editExePathButton.Height = Utils.TextBoxHeight;
            changeIconButton.Height = Utils.TextBoxHeight;

            // newTestButton.ButtonBase.Height = 30;
            // newTestButton.Grid.Height = 30;

            double rowMargin = 20;

            menuRectangle.Margin = new Thickness(Utils.TileLeftMargin + 15, Utils.MenuTopMargin, 0, 0);
            shadowRectangle.Margin = new Thickness(Utils.TileLeftMargin + 10,
                -2 * TileHeight + 29, 0, 0);

            editNameTitle.Margin = new Thickness(Utils.EditFColLeft, Utils.EditColTop, 0, 0);
            editNameTitle.Padding = new Thickness(4, 0, 0, 15);
            editNameBox.Margin = new Thickness(editNameTitle.Margin.Left,
                (int)(editNameTitle.Margin.Top + rowMargin + padding), 0, 0);

            editExePathTitle.Margin = new Thickness(editNameTitle.Margin.Left,
                (int)(editNameBox.Margin.Top * 2), 0, 0);
            editExePathTitle.Padding = new Thickness(4, 0, 0, 15);
            editExePathBox.Margin = new Thickness(editNameTitle.Margin.Left,
                (int)(editExePathTitle.Margin.Top + rowMargin + padding), 0, 0);
            editExePathButton.Margin = new Thickness(editNameTitle.Margin.Left + Utils.EditSColLeft,
                editExePathBox.Margin.Top, 0, 0);

            editPlaytimeTitle.Margin =
                new Thickness(editNameTitle.Margin.Left + Utils.EditSColLeft, editNameTitle.Margin.Top, 0, 0);
            editPlaytimeTitle.Padding = new Thickness(4, 0, 0, 15);
            editPlaytimeBox.Margin = new Thickness(editPlaytimeTitle.Margin.Left, editNameBox.Margin.Top, 0, 0);

            changeIconButton.Margin = new Thickness(0, editNameBox.Margin.Top, Utils.EditFColLeft, 0);
            editSaveButton.Margin = new Thickness(0, editExePathBox.Margin.Top, Utils.EditFColLeft, 0);

            // newTestButton.Margin = new Thickness(300, editExePathBox.Margin.Top, 0, 0);

            Thickness testButtonMargin = new Thickness(300, editExePathBox.Margin.Top, 0, 0);

            animatedElements = new List<UIElement>();
            animatedElements.AddRange(new UIElement[]
            {
                shadowRectangle, changeIconButton,
                editNameTitle, editNameBox,
                editPlaytimeTitle,
                editPlaytimeBox,
                editExePathTitle, editExePathBox, editExePathButton,
                editSaveButton,
                // newTestButton.Grid, newTestButton.ButtonBase
            });


            foreach (var element in animatedElements)
            {
                if (element is FrameworkElement frameworkElement)
                {
                    // Store the original margin in the dictionary
                    originalMargins[element] = frameworkElement.Margin;
                }
            }

            foreach (var elem in editElements)
            {
                if (elem is FrameworkElement frameworkElement)
                {
                    if (!double.IsNaN(frameworkElement.Height))
                    {
                        originalHeights[elem] = frameworkElement.Height;
                        Console.WriteLine(frameworkElement.Height);
                    }
                    else
                    {
                        originalHeights[elem] = 0;
                    }
                }
            }

            foreach (var elem in editElements)
            {
                if (elem is FrameworkElement frameworkElement)
                {
                    if (!double.IsNaN(frameworkElement.Height))
                    {
                        originalHeights[elem] = frameworkElement.Height;
                        Console.WriteLine(frameworkElement.Height);
                    }
                    else
                    {
                        originalHeights[elem] = 0;
                    }
                }
            }

            WasOpened = true;
        }

        heightAnimation.Completed += (s, a) =>
        {
            foreach (var elem in editElements)
            {
                elem.Visibility = !isMenuOpen ? Visibility.Collapsed : Visibility.Visible;
            }
        };

        // Set the visibility to visible before starting the animation if we are opening the menu
        if (isMenuOpen)
        {
            foreach (var element in editElements)
            {
                element.Visibility = Visibility.Visible;
            }
        }

        foreach (var element in editElements)
        {
            if (element is CustomButton)
            {
                Console.WriteLine("Custom Button found!");
            }

            if (animatedElements.Contains(element) && element is FrameworkElement frameworkElement)
            {
                // Get the original margin
                Thickness originalMargin = originalMargins[element];

                // Determine the target top margin based on the menu state
                double targetTopMargin =
                    isMenuOpen ? originalMargin.Top : 0; // Animate to 0 if closing, otherwise to original

                // Create the margin animation
                ThicknessAnimation marginAnimation = new ThicknessAnimation
                {
                    From = new Thickness(originalMargin.Left, isMenuOpen ? 0 : originalMargin.Top, originalMargin.Right,
                        originalMargin.Bottom),
                    To = new Thickness(originalMargin.Left, targetTopMargin, originalMargin.Right,
                        originalMargin.Bottom),
                    Duration = new Duration(TimeSpan.FromSeconds(animationDuration)),
                    FillBehavior = FillBehavior.HoldEnd,
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
                };


                // Start the margin animation

                frameworkElement.BeginAnimation(MarginProperty, marginAnimation);
            }

            double oHeight = originalHeights[element];
            DoubleAnimation heightAnimationFromOG = new DoubleAnimation
            {
                From = isMenuOpen ? 0 : oHeight, To = isMenuOpen ? oHeight : 0,
                Duration = new Duration(TimeSpan.FromSeconds(animationDuration)),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };

            // Start the other animations
            if (oHeight > 0)
            {
                element.BeginAnimation(MaxHeightProperty, heightAnimationFromOG);
            }
            else
            {
                element.BeginAnimation(MaxHeightProperty, heightAnimation);
            }

            if (element is Rectangle)
            {
                Console.WriteLine("Rectangle found");
            }

            element.BeginAnimation(OpacityProperty, opacityAnimation);
        }

        Console.WriteLine(isMenuOpen);
    }

    public void ToggleEdit_Click(object sender, RoutedEventArgs e)
    {
        // ToggleEdit();
        // EditMenu eMenu = new EditMenu(this);
        // Panel.SetZIndex(eMenu, 0);
        // Grid.SetRow(eMenu, 1);
        // grid.Children.Add(eMenu);
        // if (!grid.Children.Contains(TileEditMenu))
        // {
        //     grid.Children.Add(TileEditMenu);
        // }

        if (!TileEditMenu.IsOpen)
        {
            TileEditMenu.OpenMenu();
        }
        else
        {
            TileEditMenu.CloseMenu();
        }
        // eMenu.OpenMenu();
    }

    public void editSaveButton_Click(object sender, RoutedEventArgs e)
    {
        SaveEditedData();
    }

    // Updates elements of the tile if there is change. Has a failsafe if new time values would cause a crash
    public void SaveEditedData()
    {
        titleTextBlock.Text = editNameBox.Text;
        if (!GameName.Equals(editNameBox.Text))
        {
            GameName = editNameBox.Text;
        }

        double savedH = hTotal;
        double savedM = mTotal;
        double savedTotal = TotalPlaytime;
        Console.WriteLine(TotalPlaytime);
        try
        {
            (double hAux, double mAux) = Utils.DecodeTimeString(editPlaytimeBox.Text, hTotal, mTotal);
            if (Math.Abs(hAux - hTotal) > 0 || Math.Abs(mAux - mTotal) > 0)
            {
                TotalPlaytime = CalculatePlaytimeFromHnM(hAux, mAux);
            }

            (hTotal, mTotal) = CalculatePlaytimeFromMinutes(TotalPlaytime);
        }
        catch (FormatException)
        {
            Console.WriteLine("Format Exception");
            hTotal = savedH;
            mTotal = savedM;
            TotalPlaytime = savedTotal;
            _tileContainer.InitSave();
            MessageBox.Show("An error occured while saving new playtime");
        }

        totalPlaytime.Text = $"{hTotal}h {mTotal}m";
        editPlaytimeBox.Text = $"{hTotal}h {mTotal}m";
        Console.WriteLine("Updating bars from Tile - SaveEditedData");
        _tileContainer.UpdatePlaytimeBars();

        TextBlock mainTotalTimeBlock = Utils.mainWindow.FindName("TotalPlaytimeTextBlock") as TextBlock;
        mainTotalTimeBlock.Text = $"Total Playtime: {_tileContainer.GetTotalPlaytimePretty()}";

        _tileContainer.InitSave();
        _tileContainer.ListTiles();
        Console.WriteLine("File Saved !!!");
    }

    // Sets new path for the exe chosen by the user
    public void UpdateExe(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        // openFileDialog.InitialDirectory = Sy
        if (System.IO.Path.Exists(ExePath))
        {
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(ExePath);
        }

        openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;
            if (!ExePath.Equals(filePath))
            {
                if (_tileContainer.GetTilesExePath().Contains(filePath))
                {
                    MessageBox.Show("This executable is already in use!", "Duplicate");
                }
                else
                {
                    ExePath = filePath;
                    editExePathBox.Text = $"{ExePath}";
                    _tileContainer.InitSave();
                }
            }
        }
    }

    private String GetShortcutArguments(string shortcutPath)
    {
        if (!System.IO.File.Exists(shortcutPath))
            throw new FileNotFoundException($"Shortcut not found: {shortcutPath}");

        ShellLinkFile shortcut = ShellLinkFile.Load(shortcutPath);
        Console.WriteLine(shortcut.Arguments);

        return shortcut.Arguments;
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

            // Console.WriteLine(exePath);
            // Console.WriteLine(GetShortcutArguments("F:\\Games\\Need for Speed SHIFT\\_SHIFT.exe - Shortcut.lnk"));

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
                        PopupMenu popupMenu = new PopupMenu(text: $"Failed to start {gameName}", type: "ok");
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
                PopupMenu popupMenu = new PopupMenu(text: $"Failed to start {GameName}", type: "ok");
                popupMenu.OpenMenu();
            });
        }
    }


    private void OpenDeleteDialog(object sender, RoutedEventArgs e)
    {
        deleteMenu = new PopupMenu(text: $"Do you really want to delete {GameName}?", routedEvent1: DeleteTile);
        deleteMenu.OpenMenu();
        // DeleteTile(sender, e);
    }

    // Handles deletion of the instance
    public async void DeleteTile(object sender, RoutedEventArgs e)
    {
        // Delay for delete menu closing
        await Task.Delay(500);

        double animationDuration = 1.0; // Duration for the animations

        // Create the height animation for shrinking the tile
        DoubleAnimation heightAnimation = new DoubleAnimation
        {
            From = TileHeight * 2,
            To = 0,
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
                panel.Children.Remove(this);
            }
        };

        // Apply the animations to the tile
        editButton.BeginAnimation(MarginProperty, Utils.GetMarginTopBottomAnimation(editButton));
        removeButton.BeginAnimation(MarginProperty, Utils.GetMarginTopBottomAnimation(removeButton));
        launchButton.BeginAnimation(MarginProperty, Utils.GetMarginTopBottomAnimation(launchButton));
        BeginAnimation(MarginProperty, Utils.GetMarginTopBottomAnimation(this));
        grid.BeginAnimation(MaxHeightProperty, heightAnimation);
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

    private void SetupIconVars()
    {
        absoluteIconPath = System.IO.Path.GetFullPath(IconImagePath);

        if (!System.IO.File.Exists(absoluteIconPath))
        {
            // Handle the case where the file does not exist
            Console.WriteLine("Error: Icon file not found at " + absoluteIconPath);
            return; // Exit the method, or handle as needed
        }

        // Proceed if the file exists
        bgImageGray = Utils.ConvertToGrayscale(new BitmapImage(new Uri(absoluteIconPath, UriKind.Absolute)));
        bgImageColor = new BitmapImage(new Uri(absoluteIconPath, UriKind.Absolute));
    }

    public void UpdateImageVars(bool toSave = true)
    {
        SetupIconVars();
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
        lastTimeGradientBar.UpdateBarSizeWidth(TileWidth);

        container.Width = TileWidth;
        menuRectangle.Width = TileWidth - 30;
        shadowRectangle.Width = TileWidth - 20;
    }

    public void UpdatePlaytimeText()
    {
        totalPlaytime.Text = $"{hTotal}h {mTotal}m";
        lastPlaytime.Text = IsRunning ? $"{hLast}h {mLast}m {CurrentPlaytime}s" : $"{hLast}h {mLast}m";
    }

    /*
     * Calculates last and total hour and minute values based on the sec parameter.
     * It never goes over 59 seconds, since that is the point where the minute turns.
     * Same with the minutes-hour relation.
     * After calculating the new values, total and last playtime is updated, along with the text and bars.
     * The current seconds counter is being reset to 0.
     */
    public void CalculatePlaytimeFromSec(double sec)
    {
        int customHour = 60 - 1;
        if (sec > customHour) // 60-1
        {
            mLast++;
            mTotal++;

            LastPlaytime++;
            if (mTotal > customHour) // 60-1
            {
                hTotal++;
                mTotal = 0;
            }

            if (mLast > customHour) // 60-1
            {
                hLast++;
                mLast = 0;
                _tileContainer.InitSave();
            }

            TotalPlaytime = CalculatePlaytimeFromHnM(hTotal, mTotal);
            LastPlaytime = CalculatePlaytimeFromHnM(hLast, mLast);
            LastPlaytimePercent = TotalPlaytime > 0 ? LastPlaytime / TotalPlaytime : 0; // Avoid division by zero
            CurrentPlaytime = 0;

            UpdatePlaytimeText();
            Console.WriteLine("Updating bars from Tile - CalculatePlaytimeFromSec");
            _tileContainer.UpdatePlaytimeBars();
            _tileContainer.InitSave();
        }

        Console.WriteLine($"Current playtime of {GameName}: {hLast}h {mLast}m {CurrentPlaytime}s");
        Console.WriteLine($"Total playtime of {GameName}: {hTotal}h {mTotal}m");
    }

    // Resets all the values associated with an ongoing playtime calculation.
    public void ResetLastPlaytime()
    {
        mLast = 0;
        hLast = 0;
        CurrentPlaytime = 0;
        LastPlaytime = 0;
        LastPlaytimePercent = 0;
        UpdatePlaytimeText();
        _tileContainer.UpdateLastPlaytimeBarOfTile(Id);
        _tileContainer.InitSave();
    }

    private (double, double) CalculatePlaytimeFromMinutes(double playtime)
    {
        return ((int)(playtime / 60), (int)(playtime % 60));
    }

    private double CalculatePlaytimeFromHnM(double h, double m)
    {
        // Converting h and m to minutes
        return 60 * h + m;
    }

    public static readonly DependencyProperty GameNameProperty =
        DependencyProperty.Register("GameName", typeof(string), typeof(Tile), new PropertyMetadata(""));

    public Tile()
    {
    }

    public Tile(TileContainer tileContainer, string gameName, bool horizontalTile,
        bool horizontalEdit, bool bigBgImages, double totalTime = 0, double lastPlayedTime = 0,
        string? iconImagePath = SampleImagePath, string exePath = "", string shortcutArgs = "", double width = 760)
    {
        _tileContainer = tileContainer;
        TileWidth = width;
        TileHeight = Utils.THeight;
        CornerRadius = Utils.BorderRadius;
        TotalPlaytime = totalTime < 0 ? 0 : totalTime;
        LastPlaytime = lastPlayedTime > TotalPlaytime ? TotalPlaytime : (lastPlayedTime < 0 ? 0 : lastPlayedTime);
        LastPlaytime = lastPlayedTime;
        TotalPlaytimePercent = tileContainer.CalculateTotalPlaytime() / LastPlaytime;
        LastPlaytimePercent = Math.Round(LastPlaytime / TotalPlaytime, 2);
        GameName = gameName;
        IconImagePath = iconImagePath == null ? SampleImagePath : iconImagePath;
        if (!File.Exists(IconImagePath)) IconImagePath = SampleImagePath;

        ExePath = exePath;
        Console.WriteLine($"+!+!!+!+!+! {exePath}");
        ExePathName = System.IO.Path.GetFileNameWithoutExtension(ExePath);

        ShortcutArgs = shortcutArgs.Length > 0 ? shortcutArgs : "";

        HorizontalTileG = horizontalTile;
        HorizontalEditG = horizontalEdit;
        BigBgImages = bigBgImages;

        // Console.WriteLine($"#################### Tile Gradient settings: {HorizontalTileG}, {HorizontalEditG}");

        IsMenuToggled = false;
        WasMoved = false;

        SetupIconVars();
    }

    public void UpdateTileColors()
    {
        SetGradients();
        // gradientBrush = Utils.createLinGradBrushVer(Utils.TileColor1, Utils.TileColor2);
        // editGradientBrush = Utils.createLinGradBrushVer(Utils.EditColor1, Utils.EditColor2);
        container.Fill = gradientBrush;
        menuRectangle.Fill = editGradientBrush;
        shadowRectangle.Fill = new SolidColorBrush(Utils.ShadowColor);

        titleTextBlock.Foreground = new SolidColorBrush(Utils.FontColor);
        runningTextBlock.Foreground = new SolidColorBrush(Utils.RunningColor);

        totalPlaytimeTitle.Foreground = new SolidColorBrush(Utils.FontColor);
        totalPlaytime.Foreground = new SolidColorBrush(Utils.FontColor);
        totalTimeGradientBar.barForeground.Fill = Utils.createLinGradBrushHor(Utils.LeftColor, Utils.RightColor);

        lastPlaytimeTitle.Foreground = new SolidColorBrush(Utils.FontColor);
        lastPlaytime.Foreground = new SolidColorBrush(Utils.FontColor);
        lastTimeGradientBar.barForeground.Fill = Utils.createLinGradBrushHor(Utils.LeftColor, Utils.RightColor);

        runningTextBlock.Foreground = new SolidColorBrush(Utils.RunningColor);
        shadowRectangle.Fill = new SolidColorBrush(Utils.ShadowColor);
    }

    public void SetGradients(bool fromContainer = false)
    {
        gradientBrush = HorizontalTileG
            ? Utils.createLinGradBrushHor(Utils.TileColor1, Utils.TileColor2)
            : Utils.createLinGradBrushVer(Utils.TileColor1, Utils.TileColor2);

        editGradientBrush = HorizontalEditG
            ? Utils.createLinGradBrushHor(Utils.EditColor1, Utils.EditColor2)
            : Utils.createLinGradBrushVer(Utils.EditColor1, Utils.EditColor2);
        if (fromContainer)
        {
            container.Fill = gradientBrush;
            menuRectangle.Fill = editGradientBrush;
        }
    }

    // Sets up elements of the tile with the default values
    public void InitializeTile()
    {
        fColMarg = new[] { TileWidth * 0.25, TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin };
        sColMarg = new[] { TileWidth * 0.545, 1 };

        // gradientBrush = Utils.createLinGradBrushVer(Utils.TileColor1, Utils.TileColor2);
        // editGradientBrush = Utils.createLinGradBrushVer(Utils.EditColor1, Utils.EditColor2);

        SetGradients();

        editElements = new List<UIElement>();
        mainElements = new List<UIElement>();
        sampleTextBlock = Utils.NewTextBlock();

        sampleTextBox = Utils.NewTextBoxEdit();
        sampleTextBox.Style = (Style)Application.Current.FindResource("RoundedTextBox");

        // Create a Grid to hold the Rectangle and TextBlock
        grid = new Grid();
        (hTotal, mTotal) = CalculatePlaytimeFromMinutes(TotalPlaytime);
        (hLast, mLast) = CalculatePlaytimeFromMinutes(LastPlaytime);
        // Define the grid rows
        RowDefinition row1 = new RowDefinition();
        RowDefinition row2 = new RowDefinition();
        grid.RowDefinitions.Add(row1);
        grid.RowDefinitions.Add(row2);

        // Create the first Rectangle
        menuRectangle = new Rectangle
        {
            Width = TileWidth - 30,
            Height = TileHeight,
            RadiusX = CornerRadius,
            RadiusY = CornerRadius,
            Fill = editGradientBrush,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            MaxHeight = 0,
            Effect = Utils.dropShadowRectangle,
        };

        shadowRectangle = new Rectangle();
        shadowRectangle.Fill = new SolidColorBrush(Utils.ShadowColor);
        shadowRectangle.Width = TileWidth - 20;
        shadowRectangle.Height = TileHeight;
        shadowRectangle.MaxHeight = 0;
        shadowRectangle.HorizontalAlignment = HorizontalAlignment.Left;
        shadowRectangle.Effect = Utils.fakeShadow;

        editNameTitle = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        editNameTitle.Text = "Name";
        editNameTitle.MaxHeight = 0;
        editNameTitle.Foreground = new SolidColorBrush(Utils.DarkColor);
        editNameTitle.Effect = null;

        editNameBox = Utils.CloneTextBoxEdit(sampleTextBox);
        editNameBox.Text = GameName;
        // editNameBox.Width = 150;
        editNameBox.Effect = Utils.dropShadowLightArea;
        editNameBox.KeyDown += editBox_KeyDown;

        editPlaytimeTitle = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        editPlaytimeTitle.Text = "Playtime";
        editPlaytimeTitle.MaxHeight = 0;
        editPlaytimeTitle.Foreground = new SolidColorBrush(Utils.DarkColor);
        editPlaytimeTitle.Effect = null;

        editPlaytimeBox = Utils.CloneTextBoxEdit(sampleTextBox);
        editPlaytimeBox.Text = $"{hTotal}h {mTotal}m";
        // editPlaytimeBox.Width = 150;
        editPlaytimeBox.Height = Utils.TextBoxHeight;
        editPlaytimeBox.Effect = Utils.dropShadowLightArea;
        editPlaytimeBox.KeyDown += editBox_KeyDown;

        editExePathTitle = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        editExePathTitle.Text = "Path";
        editExePathTitle.MaxHeight = 0;
        editExePathTitle.Foreground = new SolidColorBrush(Utils.DarkColor);
        editExePathTitle.Effect = null;

        editExePathBox = Utils.CloneTextBoxEdit(sampleTextBox);
        editExePathBox.Text = $"{ExePath}";
        // editExePathBox.Width = 150;
        editExePathBox.Height = Utils.TextBoxHeight;
        editExePathBox.Effect = Utils.dropShadowLightArea;
        editExePathBox.KeyDown += editBox_KeyDown;

        editExePathButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButton"),
            Content = "Browse",
            Height = 30,
            Width = 80,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            MaxHeight = 0,
            Effect = Utils.dropShadowLightArea
        };
        editExePathButton.Click += UpdateExe;

        editSaveButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButtonSave"),
            Height = 48,
            Width = 96,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 0, 100, 0),
            MaxHeight = 0,
            Effect = Utils.dropShadowLightArea
        };
        editSaveButton.Click += editSaveButton_Click;

        changeIconButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButton"),
            Content = "Change icon",
            Height = 30,
            Width = 120,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            MaxHeight = 0,
            Effect = Utils.dropShadowLightArea
        };
        changeIconButton.Click += UpdateIcons;

        editElements.AddRange(new UIElement[]
        {
            menuRectangle, shadowRectangle, editNameTitle, editNameBox, editPlaytimeTitle,
            editSaveButton, changeIconButton,
            editPlaytimeBox,
            editExePathTitle, editExePathBox, editExePathButton,
        });
        foreach (var elem in editElements)
        {
            Grid.SetRow(elem, 1);
            grid.Children.Add(elem);
        }

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

        editButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButtonEdit"),
            Height = 40,
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, topMargin, 100, 0),
            Effect = Utils.dropShadowIcon
        };
        editButton.Click += ToggleEdit_Click;

        removeButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButtonRemove"),
            Height = 40,
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, topMargin, 50, 0),
            Effect = Utils.dropShadowIcon
        };
        removeButton.Click += OpenDeleteDialog;

        launchButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButton"),
            Content = "Launch",
            Height = 40,
            Width = 90,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 60, 50, 0),
            Effect = Utils.dropShadowIcon,
        };
        launchButton.Background = new SolidColorBrush(Colors.LightGreen);
        launchButton.Click += LaunchExe;

        mainElements.AddRange(new UIElement[] { container, editButton, removeButton, launchButton });

        Grid.SetRow(container, 0);
        Grid.SetRow(editButton, 0);
        Grid.SetRow(removeButton, 0);
        Grid.SetRow(launchButton, 0);

        Panel.SetZIndex(container, 1);
        Panel.SetZIndex(editButton, 3);
        Panel.SetZIndex(removeButton, 3);
        Panel.SetZIndex(launchButton, 3);


        grid.Children.Add(container);
        grid.Children.Add(editButton);
        grid.Children.Add(removeButton);
        grid.Children.Add(launchButton);


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

        if (!isRunning) runningTextBlock.Text = "";

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

        totalPlaytimeTitle = Utils.CloneTextBlock(sampleTextBlock);
        totalPlaytimeTitle.Text = "Total Playtime:";
        totalPlaytimeTitle.Margin =
            new Thickness(fColMarg[0], fColMarg[1] - 10, 0, 0);

        totalPlaytime = Utils.CloneTextBlock(sampleTextBlock, isBold: false);
        totalPlaytime.Text = $"{hTotal}h {mTotal}m";
        totalPlaytime.Margin = Margin =
            new Thickness(fColMarg[0], fColMarg[1] + 15, 0, 0);

        SetPlaytimeBars();

        lastPlaytimeTitle = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        lastPlaytimeTitle.Text = "Last Playtime:";
        lastPlaytimeTitle.Margin = new Thickness(sColMarg[0],
            TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin - 10, 0, 0);

        lastPlaytime = Utils.CloneTextBlock(sampleTextBlock, isBold: false);
        lastPlaytime.Text = $"{hLast}h {mLast}m";
        lastPlaytime.Margin = new Thickness(sColMarg[0],
            TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin + 15, 0, 0);

        lastTimeGradientBar = new GradientBar(this, percent: LastPlaytimePercent)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(sColMarg[0],
                TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin + 40, 0, 0),
            Effect = Utils.dropShadowText,
        };
        Panel.SetZIndex(totalPlaytimeTitle, 3);
        Panel.SetZIndex(totalPlaytime, 3);
        Panel.SetZIndex(totalTimeGradientBar, 3);

        Grid.SetRow(totalPlaytimeTitle, 0);
        Grid.SetRow(totalPlaytime, 0);
        Grid.SetRow(totalTimeGradientBar, 0);
        Grid.SetRow(lastPlaytimeTitle, 0);
        Grid.SetRow(lastPlaytime, 0);
        Grid.SetRow(lastTimeGradientBar, 0);

        grid.Children.Add(totalPlaytimeTitle);
        grid.Children.Add(totalPlaytime);
        grid.Children.Add(totalTimeGradientBar);
        grid.Children.Add(lastPlaytimeTitle);
        grid.Children.Add(lastPlaytime);
        grid.Children.Add(lastTimeGradientBar);

        Panel.SetZIndex(lastPlaytimeTitle, 3);
        Panel.SetZIndex(lastPlaytime, 3);
        Panel.SetZIndex(lastTimeGradientBar, 3);

        Panel.SetZIndex(iconContainerGrid, 2);

        // Add new EditMenu for testing purposes
        // EditMenu eMenu = new EditMenu(this);
        // Panel.SetZIndex(eMenu, 0);
        
        // grid.Children.Add(eMenu);
        TileEditMenu = new EditMenu(this);
        Grid.SetRow(TileEditMenu, 1);
        TileEditMenu.IsOpen = false;
        grid.Children.Add(TileEditMenu);
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
    public void SetPlaytimeBars()
    {
        totalTimeGradientBar = new GradientBar(this, percent: TotalPlaytimePercent)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin =
                new Thickness(fColMarg[0] - 3,
                    TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin + 40, 0, 0),
            Effect = Utils.dropShadowText,
        };

        lastTimeGradientBar = new GradientBar(this, percent: LastPlaytimePercent)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness((Utils.TextMargin + TileHeight + 20) * 2.3,
                TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin + 40, 0, 0),
            Effect = Utils.dropShadowText,
        };
    }

    private void editBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // Call your method here
            SaveEditedData();
            e.Handled = true; // Optional, prevents the beep sound
        }
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