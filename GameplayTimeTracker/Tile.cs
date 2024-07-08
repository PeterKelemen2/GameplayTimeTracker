﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameplayTimeTracker;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public class GradientBar : UserControl
{
    public double Width { get; set; }
    public double Height { get; set; }
    public double Percent { get; set; }
    public Color Color1 { get; set; }
    public Color Color2 { get; set; }
    public Color BgColor { get; set; }
    public double Padding { get; set; }
    public double Radius { get; set; }

    public GradientBar(double width, double height, double percent, Color color1, Color color2, Color bgColor,
        double padding = 5,
        double radius = 10)
    {
        Width = width;
        Height = height;
        Percent = percent;
        Color1 = color1;
        Color2 = color2;
        BgColor = bgColor;
        Padding = padding;
        Radius = radius;
        InitializeBar();
    }

    private void InitializeBar()
    {
        Grid grid = new Grid();

        LinearGradientBrush gradientBrush = new LinearGradientBrush();
        gradientBrush.StartPoint = new Point(0, 0);
        gradientBrush.EndPoint = new Point(1, 0);
        gradientBrush.GradientStops.Add(new GradientStop(Color1, 0.0));
        gradientBrush.GradientStops.Add(new GradientStop(Color2, 1.0));

        Rectangle barBackground = new Rectangle
        {
            Width = Width,
            Height = Height,
            RadiusX = Radius,
            RadiusY = Radius,
            Fill = new SolidColorBrush(BgColor)
        };

        Rectangle barForeground = new Rectangle
        {
            Width = (Width - 2 * Padding) * Percent,
            Height = Height - 2 * Padding,
            RadiusX = Radius - Padding / 2,
            RadiusY = Radius - Padding / 2,
            Fill = gradientBrush,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(Padding, 0, 0, 0)
        };

        grid.Children.Add(barBackground);
        grid.Children.Add(barForeground);

        Content = grid;
    }
}

public class Tile : UserControl
{
    private const int TextMargin = 10;
    private const int TitleFontSize = 17;
    private const int TextFontSize = 10;

    private const string SampleImagePath = "/assets/fallout3.png";

    Color DarkColor = (Color)ColorConverter.ConvertFromString("#1E2030");
    Color LightColor = (Color)ColorConverter.ConvertFromString("#2E324A");
    Color FontColor = (Color)ColorConverter.ConvertFromString("#9EABFF");
    Color LeftColor = (Color)ColorConverter.ConvertFromString("#89ACF2");
    Color RightColor = (Color)ColorConverter.ConvertFromString("#B7BDF8");
    public double TileWidth { get; set; }
    public double TileHeight { get; set; }
    public double CornerRadius { get; set; }

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(Tile), new PropertyMetadata(""));

    public Tile(double width, double height, double cornerRadius)
    {
        TileWidth = width;
        TileHeight = height;
        CornerRadius = cornerRadius;
        Text = "Fallout 3";

        Stopwatch stopwatch = Stopwatch.StartNew();
        InitializeTile();
        stopwatch.Stop();
        Console.WriteLine($"Tile initialization time: {stopwatch.Elapsed.TotalNanoseconds / 1000}");
    }

    private void InitializeTile()
    {
        // Create a Grid to hold the Rectangle and TextBlock
        Grid grid = new Grid();

        // Create a Rectangle with rounded corners
        Rectangle container = new Rectangle
        {
            Width = TileWidth,
            Height = TileHeight,
            RadiusX = CornerRadius,
            RadiusY = CornerRadius,
            Fill = new SolidColorBrush(LightColor), // Set the fill color
        };


        // Create a TextBlock for displaying text
        TextBlock titleTextBlock = new TextBlock
        {
            Text = Text, // Bind to the Text property of the UserControl
            FontWeight = FontWeights.Bold,
            FontSize = TitleFontSize,
            Foreground = new SolidColorBrush(FontColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(TextMargin, TextMargin / 2, 0, 0)
        };

        Image image = new Image
        {
            Source = new BitmapImage(new Uri(SampleImagePath, UriKind.Relative)),
            Stretch = Stretch.UniformToFill,
            Width = TileHeight / 2,
            Height = TileHeight / 2,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 20, 0, 0),
        };
        RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

        Random random = new Random();
        GradientBar gradientBar = new GradientBar(
            width: 150,
            height: 30,
            percent: random.NextDouble(),
            color1: LeftColor,
            color2: RightColor,
            bgColor: DarkColor
        )
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(TextMargin, 0, 20, TextMargin)
        };

        // Add the Rectangle and TextBlock to the Grid
        grid.Children.Add(container);
        grid.Children.Add(titleTextBlock);
        grid.Children.Add(image);
        grid.Children.Add(gradientBar);

        // Set the Grid as the content of the UserControl
        Content = grid;
    }
}