using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace GameplayTimeTracker;

public class ColorEntry : UserControl
{
    public String ColorName { get; set; }
    public String ColorValue { get; set; }
    private TextBlock nameBlock;
    public TextBlock valueBlock;
    private Rectangle picker;
    private Rectangle pickerBg;
    private Rectangle bg;
    private Grid containerGrid;
    public ColorPicker colorPicker;
    private int fontSize = 15;

    public ColorEntry()
    {
        
    }

    public ColorEntry(string colorName, string colorValue, Color c1, Color c2, double width = 250)
    {
        ColorName = colorName;
        ColorValue = colorValue;

        containerGrid = new Grid
        {
            Width = width,
            Height = 70,
            Margin = new Thickness(0, 5, 0, 5),
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        bg = new Rectangle
        {
            Fill = Utils.createLinGradBrushHor(c1, c2),
            Width = containerGrid.Width,
            Height = containerGrid.Height,
            RadiusX = 5,
            RadiusY = 5,
            Effect = Utils.dropShadowText
        };
        containerGrid.Children.Add(bg);

        nameBlock = new TextBlock
        {
            Text = Utils.GetPrettyVarName(ColorName),
            FontSize = fontSize,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Utils.FontColor),
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(10, 10, 0, 0),
        };
        containerGrid.Children.Add(nameBlock);

        valueBlock = new TextBlock
        {
            Text = colorValue,
            FontSize = fontSize,
            Foreground = new SolidColorBrush(Utils.FontColor),
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(10, 0, 0, 10),
        };
        containerGrid.Children.Add(valueBlock);
        
        colorPicker = new ColorPicker();
        colorPicker.Width = 40;
        colorPicker.Height = 40;
        colorPicker.ShowDropDownButton = false;
        colorPicker.HorizontalAlignment = HorizontalAlignment.Right;
        colorPicker.Margin = new Thickness(0, 0, 10, 0);
        colorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(colorValue);
        colorPicker.UsingAlphaChannel = false;
        colorPicker.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorValue));
        colorPicker.BorderThickness = new Thickness(0);
        colorPicker.BorderBrush = new SolidColorBrush(Utils.FontColor);
        colorPicker.Padding = new Thickness(0, colorPicker.Height, 0, 0);
        colorPicker.Effect = Utils.dropShadowText;
        containerGrid.Children.Add(colorPicker);
        
        Content = containerGrid;
    }

    private void UpdateEntryColor()
    {
        bg.Fill = Utils.createLinGradBrushHor(Utils.TileColor2, Utils.TileColor1);
    }
}