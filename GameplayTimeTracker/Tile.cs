﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;


namespace GameplayTimeTracker;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public class GradientBar : UserControl
{
    public double GWidth { get; set; }
    public double GHeight { get; set; }
    public double Percent { get; set; }
    public Color Color1 { get; set; }
    public Color Color2 { get; set; }
    public Color BgColor { get; set; }
    public double GPadding { get; set; }
    public double Radius { get; set; }

    private Rectangle barBackground;
    private Rectangle barForeground;

    public GradientBar(double gWidth, double gHeight, double percent, Color color1, Color color2, Color bgColor,
        double gPadding = 5,
        double radius = 10)
    {
        // Width = width;
        GWidth = gWidth;
        GHeight = gHeight;
        Percent = percent;
        Color1 = color1;
        Color2 = color2;
        BgColor = bgColor;
        GPadding = gPadding;
        Radius = radius;
        InitializeBar();
    }

    public void InitializeBar()
    {
        Grid grid = new Grid();

        LinearGradientBrush gradientBrush = new LinearGradientBrush();
        gradientBrush.StartPoint = new Point(0, 0);
        gradientBrush.EndPoint = new Point(1, 0);
        gradientBrush.GradientStops.Add(new GradientStop(Color1, 0.0));
        gradientBrush.GradientStops.Add(new GradientStop(Color2, 1.0));

        barBackground = new Rectangle
        {
            Width = GWidth,
            Height = GHeight,
            RadiusX = Radius,
            RadiusY = Radius,
            Fill = new SolidColorBrush(BgColor)
        };

        barForeground = new Rectangle
        {
            Width = (GWidth - 2 * GPadding) * Percent,
            Height = GHeight - 2 * GPadding,
            RadiusX = Radius - GPadding / 2,
            RadiusY = Radius - GPadding / 2,
            Fill = gradientBrush,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(GPadding, 0, 0, 0)
        };

        grid.Children.Add(barBackground);
        grid.Children.Add(barForeground);

        Content = grid;
    }
}

public class Tile : UserControl
{
    private TileContainer _tileContainer;
    private const int TextMargin = 10;
    private const int TitleFontSize = 17;
    private const int TextFontSize = 14;
    private const double THeight = 150;
    private const double BorderRadius = 10;
    private const int MenuTopMargin = -20;
    private const int TextBoxHeight = 25;
    private bool isMenuOpen = false;
    private bool wasOpened = false;

    private Grid grid;
    private Rectangle menuRectangle;
    private Rectangle container;
    private Button editButton;
    private Button removeButton;
    private Button editSaveButton;
    private Button changeIconButton;
    private Button launchButton;
    private Image image;
    private TextBlock titleTextBlock;
    private TextBlock totalPlaytimeTitle;
    private TextBlock totalPlaytime;
    private TextBlock lastPlaytimeTitle;
    private TextBlock lastPlaytime;
    private TextBox editNameBox;
    private TextBlock editNameTitle;
    private TextBox editPlaytimeBoxH;
    private TextBox editPlaytimeBoxM;
    private TextBlock editPlaytimeTitle;
    private TextBlock editPlaytimeH;
    private TextBlock editPlaytimeM;
    public GradientBar totalTimeGradientBar;
    public GradientBar lastTimeGradientBar;

    private double hTotal;
    private double mTotal;
    private double hLast;
    private double mLast;
    private double currentPlaytime;

    private const string? SampleImagePath = "assets/no_icon.png";

    Color DarkColor = (Color)ColorConverter.ConvertFromString("#1E2030");
    Color LightColor = (Color)ColorConverter.ConvertFromString("#2E324A");
    Color FontColor = (Color)ColorConverter.ConvertFromString("#C1C9FF");
    Color LeftColor = (Color)ColorConverter.ConvertFromString("#89ACF2");
    Color RightColor = (Color)ColorConverter.ConvertFromString("#B7BDF8");

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


    private double CalculateTileWidth()
    {
        return ActualWidth - (2 * 10 + 2 * SystemParameters.VerticalScrollBarWidth); // Change 10 to var
    }

    private void ToggleEdit(object sender, RoutedEventArgs e)
    {
        isMenuOpen = !isMenuOpen;
        double animationDuration = 0.15;

        DoubleAnimation heightAnimation = new DoubleAnimation
        {
            From = isMenuOpen ? 0 : TileHeight,
            To = isMenuOpen ? TileHeight : 0,
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration))
        };

        DoubleAnimation heightAnimationBox = new DoubleAnimation
        {
            From = isMenuOpen ? 0 : TextBoxHeight,
            To = isMenuOpen ? TextBoxHeight : 0,
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration))
        };

        DoubleAnimation heightAnimationButton = new DoubleAnimation
        {
            From = isMenuOpen ? 0 : 40,
            To = isMenuOpen ? 40 : 0,
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration))
        };

        DoubleAnimation opacityAnimation = new DoubleAnimation
        {
            From = isMenuOpen ? 0 : 1,
            To = isMenuOpen ? 1 : 0,
            Duration = new Duration(TimeSpan.FromSeconds(animationDuration))
        };

        if (!wasOpened)
        {
            menuRectangle.Margin = new Thickness(0, MenuTopMargin, 0, 0);
            editNameTitle.Margin = new Thickness(50, 12, 0, 0);
            editNameBox.Margin = new Thickness(100, 10, 0, 0);

            editPlaytimeTitle.Margin = new Thickness(300, 12, 0, 0);
            editPlaytimeBoxH.Margin = new Thickness(370, 10, 0, 0);
            editPlaytimeBoxM.Margin = new Thickness(440, 10, 0, 0);
            editPlaytimeH.Margin = new Thickness(425, 12, 0, 0);
            editPlaytimeM.Margin = new Thickness(495, 12, 0, 0);

            editSaveButton.Margin = new Thickness(0, 0, 60, 0);
            changeIconButton.Margin = new Thickness(50, 0, 0, 0);

            menuRectangle.MaxHeight = TileHeight;

            editNameTitle.MaxHeight = 30;
            editNameBox.Height = TextBoxHeight;
            editNameBox.MaxHeight = TextBoxHeight;

            editPlaytimeTitle.MaxHeight = 30;
            editPlaytimeBoxH.MaxHeight = TextBoxHeight;
            editPlaytimeBoxH.Height = TextBoxHeight;
            editPlaytimeBoxM.Height = TextBoxHeight;
            editPlaytimeBoxM.MaxHeight = TextBoxHeight;
            editPlaytimeH.MaxHeight = 30;
            editPlaytimeM.MaxHeight = 30;

            editSaveButton.Height = 40;
            editSaveButton.MaxHeight = 40;
            changeIconButton.MaxHeight = 30;

            wasOpened = true;
        }

        var uiElements = new List<UIElement>
        {
            menuRectangle,
            editNameTitle,
            editNameBox,
            editPlaytimeTitle,
            editPlaytimeBoxH,
            editPlaytimeH,
            editPlaytimeBoxM,
            editPlaytimeM,
            editSaveButton,
            changeIconButton
        };

        heightAnimation.Completed += (s, a) =>
        {
            if (!isMenuOpen)
            {
                foreach (var element in uiElements)
                {
                    element.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                foreach (var element in uiElements)
                {
                    element.Visibility = Visibility.Visible;
                }
            }
        };

        // Set the visibility to visible before starting the animation if we are opening the menu
        if (isMenuOpen)
        {
            foreach (var element in uiElements)
            {
                element.Visibility = Visibility.Visible;
            }
        }

        foreach (var element in uiElements)
        {
            element.BeginAnimation(HeightProperty, heightAnimation);
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

        Console.WriteLine(TotalPlaytime);
        TotalPlaytime =
            CalculatePlaytimeFromHnM(double.Parse(editPlaytimeBoxH.Text), double.Parse(editPlaytimeBoxM.Text));
        (hTotal, mTotal) = CalculatePlaytimeFromMinutes(TotalPlaytime);

        // Text = $"{hTotal}h {mTotal}m"
        totalPlaytime.Text = $"{hTotal}h {mTotal}m";
        editPlaytimeBoxH.Text = hTotal.ToString();
        editPlaytimeBoxM.Text = mTotal.ToString();
        // totalTimeGradientBar.Percent = Math.Round(TotalPlaytime / _tileContainer.CalculateTotalPlaytime(), 2);
        // totalTimeGradientBar.InitializeBar();
        _tileContainer.UpdatePlaytimeBars();
        _tileContainer.InitSave();
        _tileContainer.ListTiles();
        Console.WriteLine("File Saved !!!");
    }

    private void LaunchExe(object sender, RoutedEventArgs e)
    {
        Process.Start(ExePath);
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


    private void UpdatePlaytimeText()
    {
        totalPlaytime.Text = $"{hTotal}h {mTotal}m";
        lastPlaytime.Text = $"{hLast}h {mLast}m";
    }

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
            LastPlaytimePercent = LastPlaytime / TotalPlaytime;
            CurrentPlaytime = 0;
            UpdatePlaytimeText();
            _tileContainer.UpdatePlaytimeBars();
        }

        Console.WriteLine($"Current playtime of {GameName}: {hLast}h {mLast}m {CurrentPlaytime}s");
        Console.WriteLine($"Total playtime of {GameName}: {hTotal}h {mTotal}m");
    }

    private (double, double) CalculatePlaytimeFromMinutes(double playtime)
    {
        double h = 0;

        double aux = playtime;

        while (aux >= 60)
        {
            h += 1;
            aux -= 60;
        }

        return (h, aux);
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
        TileHeight = THeight;
        CornerRadius = BorderRadius;
        TotalPlaytime = totalTime;
        LastPlaytime = lastPlayedTime;
        LastPlaytimePercent = Math.Round(LastPlaytime / TotalPlaytime, 2);
        GameName = gameName;
        IconImagePath = iconImagePath == null ? SampleImagePath : iconImagePath;
        ExePath = exePath;

        InitializeTile();
    }

    public void InitializeTile()
    {
        // Create a Grid to hold the Rectangle and TextBlock
        grid = new Grid();
        // (double hTotal, double mTotal) = CalculatePlaytimeFromMinutes(TotalPlaytime);
        // (double hTotal, double mTotal) = CalculatePlaytimeFromMinutes(TotalPlaytime);
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
            Fill = new SolidColorBrush(RightColor),
            // Margin = new Thickness(0, MenuTopMargin, 0, 0)
            MaxHeight = 0
        };

        editNameTitle = new TextBlock
        {
            Text = "Name",
            FontWeight = FontWeights.Bold,
            FontSize = TextFontSize,
            Foreground = new SolidColorBrush(DarkColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            MaxHeight = 0
        };

        editNameBox = new TextBox
        {
            Style = (Style)Application.Current.FindResource("RoundedTextBox"),
            Text = GameName,
            Width = 150,
            MaxHeight = 0,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Left, // Center-align text horizontally
            HorizontalContentAlignment = HorizontalAlignment.Left, // Center-align content horizontally
            VerticalContentAlignment = VerticalAlignment.Center
        };

        editPlaytimeTitle = new TextBlock
        {
            Text = "Playtime",
            FontWeight = FontWeights.Bold,
            FontSize = TextFontSize,
            Foreground = new SolidColorBrush(DarkColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            MaxHeight = 0
        };

        editPlaytimeBoxH = new TextBox
        {
            Style = (Style)Application.Current.FindResource("RoundedTextBox"),
            Text = hTotal.ToString(),
            Width = 50,
            MaxHeight = 0,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Left, // Center-align text horizontally
            HorizontalContentAlignment = HorizontalAlignment.Left, // Center-align content horizontally
            VerticalContentAlignment = VerticalAlignment.Center
        };

        editPlaytimeH = new TextBlock
        {
            Text = "H",
            FontWeight = FontWeights.Bold,
            FontSize = TextFontSize,
            Foreground = new SolidColorBrush(DarkColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            MaxHeight = 0
        };

        editPlaytimeBoxM = new TextBox
        {
            Style = (Style)Application.Current.FindResource("RoundedTextBox"),
            Text = mTotal.ToString(),
            Width = 50,
            MaxHeight = 0,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Left, // Center-align text horizontally
            HorizontalContentAlignment = HorizontalAlignment.Left, // Center-align content horizontally
            VerticalContentAlignment = VerticalAlignment.Center
        };

        editPlaytimeM = new TextBlock
        {
            Text = "M",
            FontWeight = FontWeights.Bold,
            FontSize = TextFontSize,
            Foreground = new SolidColorBrush(DarkColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            MaxHeight = 0
        };

        editSaveButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButtonSave"),
            Height = 40,
            Width = 96,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 100, 0),
            MaxHeight = 0
        };
        editSaveButton.Click += SaveEditedData;

        changeIconButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButton"),
            Content = "Change icon",
            Height = 30,
            Width = 120,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            MaxHeight = 0
        };

        Grid.SetRow(menuRectangle, 1);
        Grid.SetRow(editNameTitle, 1);
        Grid.SetRow(editNameBox, 1);
        Grid.SetRow(editPlaytimeTitle, 1);
        Grid.SetRow(editPlaytimeH, 1);
        Grid.SetRow(editPlaytimeM, 1);
        Grid.SetRow(editPlaytimeBoxH, 1);
        Grid.SetRow(editPlaytimeBoxM, 1);
        Grid.SetRow(editSaveButton, 1);
        Grid.SetRow(changeIconButton, 1);
        grid.Children.Add(menuRectangle);
        grid.Children.Add(editNameTitle);
        grid.Children.Add(editNameBox);
        grid.Children.Add(editPlaytimeTitle);
        grid.Children.Add(editPlaytimeH);
        grid.Children.Add(editPlaytimeM);
        grid.Children.Add(editPlaytimeBoxH);
        grid.Children.Add(editPlaytimeBoxM);
        grid.Children.Add(editSaveButton);
        grid.Children.Add(changeIconButton);

        // Create the second Rectangle
        container = new Rectangle
        {
            Width = TileWidth,
            Height = TileHeight,
            RadiusX = CornerRadius,
            RadiusY = CornerRadius,
            Fill = new SolidColorBrush(LightColor),
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
        };
        launchButton.Click += LaunchExe;

        Grid.SetRow(container, 0);
        Grid.SetRow(editButton, 0);
        Grid.SetRow(removeButton, 0);
        Grid.SetRow(launchButton, 0);
        grid.Children.Add(container);
        grid.Children.Add(editButton);
        grid.Children.Add(removeButton);
        grid.Children.Add(launchButton);

        // Create a TextBlock for displaying text
        titleTextBlock = new TextBlock
        {
            Text = GameName,
            FontWeight = FontWeights.Bold,
            FontSize = TitleFontSize,
            Foreground = new SolidColorBrush(FontColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(TextMargin * 2, TextMargin / 2, 0, 0)
        };

        // Add the TextBlock to the grid
        Grid.SetRow(titleTextBlock, 0); // Position it in the second row with the container
        grid.Children.Add(titleTextBlock);

        // Create the Image and other UI elements, positioning them in the second row as well
        if (IconImagePath != null)
        {
            string absoluteIconPath = System.IO.Path.GetFullPath(IconImagePath);
            image = new Image
            {
                Source = new BitmapImage(new Uri(absoluteIconPath, UriKind.Absolute)),
                Stretch = Stretch.UniformToFill,
                Width = TileHeight / 2,
                Height = TileHeight / 2,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(50, TileHeight / 2 - TitleFontSize - TextMargin, 0, 0),
            };
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
        }
        else
        {
            Console.WriteLine("Icon was null");
        }


        // Add all other elements as before, positioning them in the second row
        Grid.SetRow(image, 0);
        grid.Children.Add(image);

        // Add playtime elements
        totalPlaytimeTitle = new TextBlock
        {
            Text = "Total Playtime:",
            FontWeight = FontWeights.Bold,
            FontSize = TextFontSize,
            Foreground = new SolidColorBrush(FontColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(TextMargin + TileHeight + 20, TileHeight / 2 - TitleFontSize - TextMargin - 10, 0, 0)
        };

        totalPlaytime = new TextBlock
        {
            Text = $"{hTotal}h {mTotal}m",
            FontWeight = FontWeights.Normal,
            FontSize = TextFontSize,
            Foreground = new SolidColorBrush(FontColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(TextMargin + TileHeight + 20, TileHeight / 2 - TitleFontSize - TextMargin + 15, 0, 0)
        };

        totalTimeGradientBar = new GradientBar(
            gWidth: 150,
            gHeight: 30,
            percent: TotalPlaytimePercent,
            color1: LeftColor,
            color2: RightColor,
            bgColor: DarkColor
        )
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(TextMargin + TileHeight + 20, TileHeight / 2 - TitleFontSize - TextMargin + 40, 0, 0)
        };

        lastPlaytimeTitle = new TextBlock
        {
            Text = "Last Playtime:",
            FontWeight = FontWeights.Bold,
            FontSize = TextFontSize,
            Foreground = new SolidColorBrush(FontColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness((TextMargin + TileHeight + 20) * 2.3,
                TileHeight / 2 - TitleFontSize - TextMargin - 10, 0, 0)
        };

        lastPlaytime = new TextBlock
        {
            Text = $"{hLast}h {mLast}m",
            FontWeight = FontWeights.Normal,
            FontSize = TextFontSize,
            Foreground = new SolidColorBrush(FontColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness((TextMargin + TileHeight + 20) * 2.3,
                TileHeight / 2 - TitleFontSize - TextMargin + 15, 0, 0)
        };

        lastTimeGradientBar = new GradientBar(
            gWidth: 150,
            gHeight: 30,
            percent: LastPlaytimePercent,
            color1: LeftColor,
            color2: RightColor,
            bgColor: DarkColor
        )
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness((TextMargin + TileHeight + 20) * 2.3,
                TileHeight / 2 - TitleFontSize - TextMargin + 40, 0, 0)
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
}