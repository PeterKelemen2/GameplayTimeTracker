namespace GameplayTimeTracker;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System;
using System.Diagnostics;
using System.Windows;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

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

    public bool WasInitialized { get; set; }

    private Rectangle barBackground;
    public Rectangle barForeground;
    private LinearGradientBrush gradientBrush;
    private Grid grid = new();
    private Tile TileParent;

    public GradientBar(Tile tileParent,
        double percent,
        double gWidth = 150,
        double gHeight = 30,
        double gPadding = 5,
        double radius = 10)
    {
        // Width = width;
        TileParent = tileParent;
        GWidth = TileParent.TileWidth * 0.25;
        // GWidth = gWidth;
        GHeight = gHeight;
        Percent = percent;
        Color1 = Utils.LeftColor;
        Color2 = Utils.RightColor;
        BgColor = Utils.DarkColor;
        GPadding = gPadding;
        Radius = radius;

        gradientBrush = new LinearGradientBrush();
        gradientBrush.StartPoint = new Point(0, 0);
        gradientBrush.EndPoint = new Point(1, 0);
        gradientBrush.GradientStops.Add(new GradientStop(Color1, 0.0));
        gradientBrush.GradientStops.Add(new GradientStop(Color2, 1.0));
        gradientBrush.Freeze();

        InitializeBar();
    }

    public void UpdateBarSizeWidth(double newWidth)
    {
        GWidth = newWidth * 0.25;
        barBackground.Width = GWidth;
        UpdateBar();
    }
    
    private double CalculateWidth()
    {
        Percent = Percent > 1 ? 1 : (Percent < 0 ? 0 : Percent);
        return (GWidth - (2 * GPadding)) * Percent;
    }

    // Calculates new width needed for bar. If I change any of this, it's not working for some reason 
    public void UpdateBar()
    {
        // Console.WriteLine("Changing bar size to " + Percent);
        // double newWidth = CalculateWidth();
        barForeground.Width = CalculateWidth();
    }

    // Sets up bar with initial values
    public void InitializeBar()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        barBackground = new Rectangle
        {
            Width = GWidth,
            Height = GHeight,
            RadiusX = Radius,
            RadiusY = Radius,
            Fill = new SolidColorBrush(BgColor),
        };

        double radiusValue = Radius - GPadding / 2;
        barForeground = new Rectangle
        {
            Width = CalculateWidth(),
            Height = GHeight - 2 * GPadding,
            RadiusX = radiusValue,
            RadiusY = radiusValue,
            Fill = gradientBrush,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(GPadding, 0, 0, 0)
        };

        grid.Children.Add(barBackground);
        grid.Children.Add(barForeground);

        Content = grid;

        stopwatch.Stop();
        WasInitialized = true;
    }
}