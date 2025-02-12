using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace GameplayTimeTracker.Menu.Content;

public class ColorEntry : UserControl
{
    public String ColorName { get; set; }
    public String ColorValue { get; set; }
    public TextBlock nameBlock;
    public TextBlock valueBlock;
    private Rectangle picker;
    private Rectangle pickerBg;
    public Rectangle bg;
    private Grid containerGrid;
    public ColorPicker colorPicker;
    private int fontSize = 16;

    public ColorEntry()
    {
    }

    public ColorEntry(string colorName, string colorValue, Color c1, Color c2,
        double width = 370)
    {
        ColorName = colorName;
        ColorValue = colorValue;

        containerGrid = new Grid
        {
            Width = width,
            Height = 70,
            Margin = new Thickness(0, 5, 0, 5),
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        containerGrid.CacheMode = new BitmapCache();

        bg = new Rectangle
        {
            Width = containerGrid.Width,
            Height = containerGrid.Height,
            RadiusX = 5,
            RadiusY = 5,
            Effect = AppEffects.dropShadowText
        };
        BindingHelper.SetGradientColorBinding(bg, Shape.FillProperty, "Card 1", "Card 2", true);
        containerGrid.Children.Add(bg);

        nameBlock = new TextBlock
        {
            Text = colorName,
            FontSize = fontSize,
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(10, 10, 0, 0),
        };
        BindingHelper.SetColorBinding(nameBlock, ForegroundProperty, "Font");
        containerGrid.Children.Add(nameBlock);

        valueBlock = new TextBlock
        {
            Text = colorValue,
            FontSize = fontSize,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(10, 0, 0, 10),
        };
        BindingHelper.SetColorBinding(valueBlock, ForegroundProperty, "Font");
        containerGrid.Children.Add(valueBlock);

        colorPicker = new ColorPicker();
        colorPicker.Width = 40;
        colorPicker.Height = 40;
        colorPicker.ShowDropDownButton = false;
        colorPicker.HorizontalAlignment = HorizontalAlignment.Right;
        colorPicker.Margin = new Thickness(0, 0, 10, 0);
        colorPicker.UsingAlphaChannel = false;
        colorPicker.BorderThickness = new Thickness(0);
        colorPicker.Padding = new Thickness(0, colorPicker.Height, 0, 0);
        colorPicker.Effect = AppEffects.dropShadowText;

        Binding newBinding = new Binding
        {
            Source = Common.Settings.CurrentTheme.Colors,
            Path = new PropertyPath($"[{colorName}]"),
            Mode = BindingMode.TwoWay,
        };

        BindingOperations.SetBinding(colorPicker, ColorPicker.SelectedColorProperty, newBinding);
        BindingHelper.SetColorBinding(colorPicker, BackgroundProperty, colorName);

        Console.WriteLine($"Color picker value: {Common.Settings.CurrentTheme.Colors[colorName]}");
        containerGrid.Children.Add(colorPicker);

        Content = containerGrid;
    }
}