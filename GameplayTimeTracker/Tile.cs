﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mime;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Microsoft.Win32;
using Atk;
using Application = System.Windows.Application;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
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
    private bool wasOpened = false;
    public bool isRunning = false;
    private bool isRunningGame = false;
    public bool wasRunning = false;

    private Grid grid;
    private Rectangle menuRectangle;
    private Rectangle shadowRectangle;
    private Rectangle container;
    private Button editButton;
    private Button removeButton;
    private Button editSaveButton;
    private Button changeIconButton;
    private Button launchButton;
    private Image image;
    public Image bgImage;
    private Grid containerGrid;
    private TextBlock titleTextBlock;
    public TextBlock runningTextBlock;
    private TextBlock totalPlaytimeTitle;
    private TextBlock totalPlaytime;
    private TextBlock lastPlaytimeTitle;
    private TextBlock lastPlaytime;
    private TextBox editNameBox;
    private TextBlock editNameTitle;
    private TextBox editPlaytimeBoxH;
    private TextBox editPlaytimeBoxM;
    private TextBox editPlaytimeBox;
    private TextBlock editPlaytimeTitle;
    private TextBlock editPlaytimeH;
    private TextBlock editPlaytimeM;
    public GradientBar totalTimeGradientBar;
    public GradientBar lastTimeGradientBar;
    public BitmapSource bgImageGray;
    public BitmapSource bgImageColor;
    private string absoluteIconPath;

    List<UIElement> editElements = new List<UIElement>();
    List<UIElement> mainElements = new List<UIElement>();
    List<UIElement> animatedElements = new List<UIElement>();

    private double hTotal;
    private double mTotal;
    private double hLast;
    private double mLast;
    private double currentPlaytime;

    private const string? SampleImagePath = "assets/no_icon.png";
    private const string? AssetsFolder = "assets";

    public int Id { get; set; }
    public double TileWidth { get; set; }
    public double TileHeight { get; set; }
    public double CornerRadius { get; set; }

    public double TotalPlaytime { get; set; }
    public double TotalPlaytimePercent { get; set; }
    public double LastPlaytime { get; set; }
    public double LastPlaytimePercent { get; set; }
    public string? IconImagePath { get; set; }
    public string ExePath { get; set; }
    public double CurrentPlaytime { get; set; }
    public double HTotal { get; set; }
    public double HLast { get; set; }
    public double MTotal { get; set; }
    public double MLast { get; set; }
    public bool IsRunning { get; set; }
    public bool IsMenuToggled { get; set; }

    public bool IsRunningGame { get; set; }
    public bool WasRunning { get; set; }

    private Dictionary<UIElement, Thickness> originalMargins = new();
    private bool wasOnceOpened = false;

    //TODO: Put shadow under the tile if toggled
    private void ToggleEdit(object sender, RoutedEventArgs e)
    {
        isMenuOpen = !isMenuOpen;
        IsMenuToggled = !IsMenuToggled;
        isMenuOpen = IsMenuToggled;
        double animationDuration = 0.15;


        DoubleAnimation heightAnimation = new DoubleAnimation
        {
            From = isMenuOpen ? 0 : TileHeight, To = isMenuOpen ? TileHeight : 0,
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration))
        };

        DoubleAnimation opacityAnimation = new DoubleAnimation
        {
            From = isMenuOpen ? 0 : 1, To = isMenuOpen ? 1 : 0,
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration))
        };

        if (!wasOpened)
        {
            menuRectangle.Margin = new Thickness(Utils.TileLeftMargin + 15, Utils.MenuTopMargin, 0, 0);
            shadowRectangle.Margin = new Thickness(Utils.TileLeftMargin + 10, -2 * TileHeight + 30, 0, 0);
            editNameTitle.Margin = new Thickness(50, Utils.EditFirstRowTopMargin, 0, 0);
            editNameBox.Margin = new Thickness(100, Utils.EditFirstRowTopMargin, 0, 0);

            editPlaytimeTitle.Margin = new Thickness(300, Utils.EditFirstRowTopMargin, 0, 0);
            editPlaytimeBoxH.Margin = new Thickness(370, Utils.EditFirstRowTopMargin, 0, 0);
            editPlaytimeBoxM.Margin = new Thickness(440, Utils.EditFirstRowTopMargin, 0, 0);
            editPlaytimeH.Margin = new Thickness(425, Utils.EditFirstRowTopMargin, 0, 0);
            editPlaytimeM.Margin = new Thickness(495, Utils.EditFirstRowTopMargin, 0, 0);

            editPlaytimeBox.Margin = new Thickness(370, Utils.EditSecondRowTopMargin, 0, 0);

            editSaveButton.Margin = new Thickness(0, 0, 60, 0);
            changeIconButton.Margin = new Thickness(50, Utils.EditSecondRowTopMargin, 0, 0);

            menuRectangle.MaxHeight = TileHeight;
            shadowRectangle.MaxHeight = 30;

            editNameTitle.MaxHeight = Utils.EditTextMaxHeight;
            editNameBox.Height = Utils.TextBoxHeight;
            editNameBox.MaxHeight = Utils.TextBoxHeight;
            editPlaytimeBox.MaxHeight = Utils.EditTextMaxHeight;

            // editNameTitle, editPlaytimeBox, editNameBox, editPlaytimeTitle, editPlaytimeBoxH, editPlaytimeBoxM, editPlaytimeH, editPlaytimeM

            editPlaytimeTitle.MaxHeight = Utils.EditTextMaxHeight;
            editPlaytimeBoxH.MaxHeight = Utils.TextBoxHeight;
            editPlaytimeBoxH.Height = Utils.TextBoxHeight;
            editPlaytimeBoxM.Height = Utils.TextBoxHeight;
            editPlaytimeBoxM.MaxHeight = Utils.TextBoxHeight;
            editPlaytimeH.MaxHeight = Utils.EditTextMaxHeight;
            editPlaytimeM.MaxHeight = Utils.EditTextMaxHeight;

            editSaveButton.Height = 40;
            editSaveButton.MaxHeight = 40;
            changeIconButton.MaxHeight = Utils.EditTextMaxHeight;

            wasOpened = true;

            if (!wasOnceOpened)
            {
                animatedElements = new List<UIElement>();
                animatedElements.AddRange(new UIElement[]
                {
                    shadowRectangle, changeIconButton,
                    editNameTitle, editNameBox,
                    editPlaytimeTitle, editPlaytimeBoxH, editPlaytimeH, editPlaytimeBoxM, editPlaytimeM,
                    editPlaytimeBox
                });
                foreach (var element in animatedElements)
                {
                    if (element is FrameworkElement frameworkElement)
                    {
                        // Store the original margin in the dictionary
                        originalMargins[element] = frameworkElement.Margin;
                    }
                }

                wasOnceOpened = true;
            }
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
                    FillBehavior = FillBehavior.HoldEnd // Holds the end value after the animation completes
                };

                // Start the margin animation
                frameworkElement.BeginAnimation(MarginProperty, marginAnimation);
            }

            // Start the other animations
            element.BeginAnimation(MaxHeightProperty, heightAnimation);
            element.BeginAnimation(OpacityProperty, opacityAnimation);
        }

        Console.WriteLine(isMenuOpen);
    }

    public void SaveEditedData(object sender, RoutedEventArgs e)
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
            // TotalPlaytime =
            //     CalculatePlaytimeFromHnM(double.Parse(editPlaytimeBoxH.Text), double.Parse(editPlaytimeBoxM.Text));
            (double hAux, double mAux) = Utils.DecodeTimeString(editPlaytimeBox.Text, hTotal, mTotal);
            if (Math.Abs(hAux - hTotal) > 0 || Math.Abs(mAux - mTotal) > 0)
            {
                TotalPlaytime = CalculatePlaytimeFromHnM(hAux, mAux);
            }

            // TotalPlaytime = CalculatePlaytimeFromHnM(hAux, mAux);
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
            // editPlaytimeBoxH.Text = hTotal.ToString();
            // editPlaytimeBoxM.Text = mTotal.ToString();
        }

        // MessageBox.Show(Utils.DecodeTimeString(editPlaytimeBox.Text).ToString());

        totalPlaytime.Text = $"{hTotal}h {mTotal}m";
        editPlaytimeBox.Text = $"{hTotal}h {mTotal}m";
        // editPlaytimeBoxH.Text = hTotal.ToString();
        // editPlaytimeBoxM.Text = mTotal.ToString();
        _tileContainer.UpdatePlaytimeBars();
        _tileContainer.InitSave();
        _tileContainer.ListTiles();
        Console.WriteLine("File Saved !!!");
    }

    private void LaunchExe(object sender, RoutedEventArgs e)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = ExePath,
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }
        catch (Win32Exception win32Ex) when (win32Ex.NativeErrorCode == 740) // Error 740 means elevation required
        {
            // Prompt user for admin if the error suggests elevation is needed
            MessageBoxResult result = MessageBox.Show(
                $"The application {GameName} requires administrator privileges. Do you want to run it as administrator?",
                "Elevation Required", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Re-launch with admin privileges
                var startInfo = new ProcessStartInfo
                {
                    FileName = ExePath,
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(startInfo);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            MessageBox.Show($"Could not launch {GameName}\n\n{ex.Message}", "Something went wrong!",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void OpenDeleteDialog(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {GameName} from the library?",
            "Delete Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            DeleteTile();
            _tileContainer.InitSave();
            _tileContainer.ListTiles();
        }
    }

    public void DeleteTile()
    {
        double animationDuration = 0.2;

        DoubleAnimation heightAnimation = new DoubleAnimation
        {
            From = TileHeight,
            To = 0,
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration))
        };

        DoubleAnimation opacityAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration))
        };

        heightAnimation.Completed += (s, a) =>
        {
            // Remove the tile from the container and the parent after the animation completes
            _tileContainer.RemoveTileById(Id);
            _tileContainer.UpdatePlaytimeBars();
            if (Parent is Panel panel)
            {
                panel.Children.Remove(this);
            }
        };

        // Apply the animations to the tile
        BeginAnimation(HeightProperty, heightAnimation);
        BeginAnimation(OpacityProperty, opacityAnimation);
    }


    public string GameName
    {
        get { return (string)GetValue(GameNameProperty); }
        set { SetValue(GameNameProperty, value); }
    }

    public void UpdateIcons(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter =
            "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Executable files (*.exe)|*.exe|All files (*.*)|*.*";

        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;
            string fileName = System.IO.Path.GetFileName(filePath);
            fileName = fileName.Substring(0, fileName.Length - 4);

            string currentDirectory = Directory.GetCurrentDirectory();
            string potentialImagePath = System.IO.Path.Combine(currentDirectory, AssetsFolder, $"{fileName}.png");

            if (!Equals(filePath, potentialImagePath))
            {
                string uniqueFileName = $"{GameName}-{Guid.NewGuid().ToString()}.png";
                string? iconPath = $"assets/{uniqueFileName}";

                Utils.PrepIcon(filePath, iconPath);
                iconPath = Utils.IsValidImage(iconPath) ? iconPath : SampleImagePath;
                IconImagePath = iconPath;
            }
            else
            {
                // IconImagePath = potentialImagePath;
                IconImagePath = System.IO.Path.Combine(AssetsFolder, $"{fileName}.png");
            }

            UpdateImageVars();
        }
    }

    private void SetupIconVars()
    {
        absoluteIconPath = System.IO.Path.GetFullPath(IconImagePath);
        bgImageGray = Utils.ConvertToGrayscale(new BitmapImage(new Uri(absoluteIconPath, UriKind.Absolute)));
        bgImageColor = new BitmapImage(new Uri(absoluteIconPath, UriKind.Absolute));
    }

    private void UpdateImageVars()
    {
        SetupIconVars();
        bgImage.Source = bgImageGray;
        image.Source = bgImageColor;
        ToggleBgImageColor(IsRunning);
        _tileContainer.InitSave();
        Console.WriteLine($"Icon for {GameName} changed to {absoluteIconPath}");
    }

    public void UpdateTileWidth(double newWidth)
    {
        TileWidth = newWidth;
        container.Width = TileWidth;
        menuRectangle.Width = TileWidth - 30;
        shadowRectangle.Width = TileWidth - 20;
    }

    public void UpdatePlaytimeText()
    {
        totalPlaytime.Text = $"{hTotal}h {mTotal}m";
        lastPlaytime.Text = $"{hLast}h {mLast}m";
    }

    public void UpdateEditPlaytimeText()
    {
        if (!(editPlaytimeBoxH.IsFocused || editPlaytimeBoxM.IsFocused))
        {
            editPlaytimeBoxH.Text = $"{hTotal}";
            editPlaytimeBoxM.Text = $"{mTotal}";
        }
    }

    public void CalculatePlaytimeFromSec(double sec, bool resetCurrent = false)
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
            _tileContainer.UpdatePlaytimeBars();
            _tileContainer.InitSave();
        }

        Console.WriteLine($"Current playtime of {GameName}: {hLast}h {mLast}m {CurrentPlaytime}s");
        Console.WriteLine($"Total playtime of {GameName}: {hTotal}h {mTotal}m");
    }

    public void ResetLastPlaytime()
    {
        mLast = 0;
        hLast = 0;
        CurrentPlaytime = 0;
        LastPlaytime = 0;
        LastPlaytimePercent = 0;
        UpdatePlaytimeText();
        // _tileContainer.UpdatePlaytimeBars();
        _tileContainer.UpdateLastPlaytimeBarOfTile(Id);
        _tileContainer.InitSave();
    }

    private (double, double) CalculatePlaytimeFromMinutes(double playtime)
    {
        return ((int)(playtime / 60), (int)(playtime % 60));
    }

    private double CalculatePlaytimeFromHnM(double h, double m)
    {
        return 60 * h + m;
    }

    public static readonly DependencyProperty GameNameProperty =
        DependencyProperty.Register("GameName", typeof(string), typeof(Tile), new PropertyMetadata(""));


    public Tile(TileContainer tileContainer, string gameName, double totalTime = 20, double lastPlayedTime = 10,
        string? iconImagePath = SampleImagePath, string exePath = "", double width = 740)
    {
        _tileContainer = tileContainer;
        TileWidth = width;
        TileHeight = Utils.THeight;
        CornerRadius = Utils.BorderRadius;
        TotalPlaytime = totalTime;
        LastPlaytime = lastPlayedTime;
        //TODO: Handle new last play
        // LastPlaytime = 0;
        LastPlaytimePercent = Math.Round(LastPlaytime / TotalPlaytime, 2);
        // LastPlaytimePercent = 0;
        GameName = gameName;
        IconImagePath = iconImagePath == null ? SampleImagePath : iconImagePath;
        ExePath = exePath;
        IsMenuToggled = false;

        SetupIconVars();
        // absoluteIconPath = System.IO.Path.GetFullPath(IconImagePath);
        // bgImageGray = Utils.ConvertToGrayscale(new BitmapImage(new Uri(absoluteIconPath, UriKind.Absolute)));
        // bgImageColor = new BitmapImage(new Uri(absoluteIconPath, UriKind.Absolute));


        InitializeTile();
    }

    public void InitializeTile()
    {
        LinearGradientBrush gradientBrush = new LinearGradientBrush();
        gradientBrush.StartPoint = new Point(0, 0);
        gradientBrush.EndPoint = new Point(1, 0);
        gradientBrush.GradientStops.Add(new GradientStop(Utils.TileColor1, 0.0));
        gradientBrush.GradientStops.Add(new GradientStop(Utils.TileColor2, 1.0));
        gradientBrush.Freeze();

        editElements = new List<UIElement>();
        mainElements = new List<UIElement>();
        var sampleTextBlock = Utils.NewTextBlock();


        var sampleTextBox = Utils.NewTextBoxEdit();
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
            Fill = new SolidColorBrush(Utils.RightColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            MaxHeight = 0,
            Effect = Utils.dropShadowRectangle,
        };

        shadowRectangle = new Rectangle();
        shadowRectangle.Fill = new SolidColorBrush(Utils.ShadowColor);
        shadowRectangle.Width = TileWidth - 20;
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
        editNameBox.Width = 150;
        editNameBox.Effect = Utils.dropShadowLightArea;


        editPlaytimeTitle = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        editPlaytimeTitle.Text = "Playtime";
        editPlaytimeTitle.MaxHeight = 0;
        editPlaytimeTitle.Foreground = new SolidColorBrush(Utils.DarkColor);
        editPlaytimeTitle.Effect = null;

        editPlaytimeBoxH = Utils.CloneTextBoxEdit(sampleTextBox);
        editPlaytimeBoxH.Text = hTotal.ToString();
        editPlaytimeBoxH.Effect = Utils.dropShadowLightArea;

        editPlaytimeH = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        editPlaytimeH.Text = "H";
        editPlaytimeH.MaxHeight = 0;
        editPlaytimeH.Foreground = new SolidColorBrush(Utils.DarkColor);
        editPlaytimeH.Effect = null;

        editPlaytimeBoxM = Utils.CloneTextBoxEdit(sampleTextBox);
        editPlaytimeBoxM.Text = mTotal.ToString();
        editPlaytimeBoxM.Effect = Utils.dropShadowLightArea;

        editPlaytimeM = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        editPlaytimeM.Text = "M";
        editPlaytimeM.MaxHeight = 0;
        editPlaytimeM.Foreground = new SolidColorBrush(Utils.DarkColor);
        editPlaytimeM.Effect = null;

        editPlaytimeBox = Utils.CloneTextBoxEdit(sampleTextBox);
        editPlaytimeBox.Width = 200;


        editSaveButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButtonSave"),
            Height = 40,
            Width = 96,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 100, 0),
            MaxHeight = 0,
            Effect = Utils.dropShadowIcon
        };
        editSaveButton.Click += SaveEditedData;

        changeIconButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButton"),
            Content = "Change icon",
            Height = 30,
            Width = 120,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            MaxHeight = 0,
            Effect = Utils.dropShadowIcon
        };
        changeIconButton.Click += UpdateIcons;

        editElements.AddRange(new UIElement[]
        {
            menuRectangle, shadowRectangle, editNameTitle, editNameBox, editPlaytimeTitle,
            editPlaytimeH, editPlaytimeM, editPlaytimeBoxH, editPlaytimeBoxM, editSaveButton, changeIconButton,
            editPlaytimeBox
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
            HorizontalAlignment = HorizontalAlignment.Left
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
        editButton.Click += ToggleEdit;

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
        grid.Children.Add(container);
        grid.Children.Add(editButton);
        grid.Children.Add(removeButton);
        grid.Children.Add(launchButton);

        // Create a TextBlock for displaying text

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
        Panel.SetZIndex(titleTextBlock, 1);
        Panel.SetZIndex(runningTextBlock, 1);

        if (!isRunning)
        {
            runningTextBlock.Text = "";
        }
        else
        {
            isRunningGame = true;
        }

        // Create the Image and other UI elements, positioning them in the second row as well
        if (IconImagePath != null)
        {
            double imageScale = 1.5;
            containerGrid = new Grid
            {
                Width = TileHeight * imageScale + 20,
                Height = TileHeight,
                ClipToBounds = true,
                Margin = new Thickness(10, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            bgImage = new Image
            {
                Source = bgImageGray,
                Stretch = Stretch.UniformToFill,
                Width = TileHeight * imageScale,
                Height = TileHeight * imageScale,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Effect = Utils.blurEffect,
                Opacity = 0.7
            };

            image = new Image
            {
                Source = new BitmapImage(new Uri(absoluteIconPath, UriKind.Absolute)),
                Stretch = Stretch.UniformToFill,
                Width = TileHeight / 2,
                Height = TileHeight / 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(-10, 20, 10, 0),
            };

            var imageBorder = new Border
            {
                Padding = new Thickness(20),
                Child = image,
                Effect = Utils.dropShadowIcon,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var bgImageBorder = new Border
            {
                Padding = new Thickness(20, 0, 20, 0),
                Height = TileHeight * 2,
                Width = TileHeight * 2,
                Child = bgImage,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            containerGrid.Children.Add(bgImageBorder);
            containerGrid.Children.Add(imageBorder);
        }
        else
        {
            Console.WriteLine("Icon was null");
        }


        // Add all other elements as before, positioning them in the second row
        // Grid.SetRow(image, 0);
        // Grid.SetRow(bgImage, 0);
        Grid.SetRow(containerGrid, 0);
        // grid.Children.Add(bgImage);
        grid.Children.Add(containerGrid);
        // Add playtime elements


        double[] fColMarg =
            { Utils.TextMargin + TileHeight + 20, TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin };
        totalPlaytimeTitle = Utils.CloneTextBlock(sampleTextBlock);
        totalPlaytimeTitle.Text = "Total Playtime:";
        totalPlaytimeTitle.Margin =
            new Thickness(fColMarg[0], fColMarg[1] - 10, 0, 0);

        totalPlaytime = Utils.CloneTextBlock(sampleTextBlock, isBold: false);
        totalPlaytime.Text = $"{hTotal}h {mTotal}m";
        totalPlaytime.Margin = Margin =
            new Thickness(fColMarg[0], fColMarg[1] + 15, 0, 0);

        totalTimeGradientBar = new GradientBar(percent: TotalPlaytimePercent)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin =
                new Thickness(Utils.TextMargin + TileHeight + 20,
                    TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin + 40, 0, 0),
            Effect = Utils.dropShadowText,
        };

        lastPlaytimeTitle = Utils.CloneTextBlock(sampleTextBlock, isBold: true);
        lastPlaytimeTitle.Text = "Last Playtime:";
        lastPlaytimeTitle.Margin = new Thickness((Utils.TextMargin + TileHeight + 20) * 2.3,
            TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin - 10, 0, 0);

        lastPlaytime = Utils.CloneTextBlock(sampleTextBlock, isBold: false);
        lastPlaytime.Text = $"{hLast}h {mLast}m";
        lastPlaytime.Margin = new Thickness((Utils.TextMargin + TileHeight + 20) * 2.3,
            TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin + 15, 0, 0);

        lastTimeGradientBar = new GradientBar(percent: LastPlaytimePercent)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness((Utils.TextMargin + TileHeight + 20) * 2.3,
                TileHeight / 2 - Utils.TitleFontSize - Utils.TextMargin + 40, 0, 0),
            Effect = Utils.dropShadowText,
        };

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

        // Set the Grid as the content of the UserControl
        Content = grid;
    }

    public void ToggleBgImageColor(bool runningBool)
    {
        bgImage.Source = runningBool ? bgImageColor : bgImageGray;
    }
}