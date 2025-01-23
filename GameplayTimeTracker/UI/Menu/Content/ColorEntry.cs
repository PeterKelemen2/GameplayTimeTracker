using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace GameplayTimeTracker.Menu.Content;

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
    private int fontSize = 16;

    public ColorEntry()
    {
    }

    public ColorEntry(string colorName, string colorValue, Color c1, Color c2, double width = 370)
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

        bg = new Rectangle
        {
            Fill = ColorHelper.CreateLinGradBrushHor(c1, c2),
            Width = containerGrid.Width,
            Height = containerGrid.Height,
            RadiusX = 5,
            RadiusY = 5,
            Effect = AppEffects.dropShadowText
        };
        containerGrid.Children.Add(bg);

        nameBlock = new TextBlock
        {
            Text = colorName,
            FontSize = fontSize,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(AppColors.Font),
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(10, 10, 0, 0),
        };
        containerGrid.Children.Add(nameBlock);

        valueBlock = new TextBlock
        {
            Text = colorValue,
            FontSize = fontSize,
            Foreground = new SolidColorBrush(AppColors.Font),
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
        colorPicker.BorderBrush = new SolidColorBrush(AppColors.Font);
        colorPicker.Padding = new Thickness(0, colorPicker.Height, 0, 0);
        colorPicker.Effect = AppEffects.dropShadowText;
        containerGrid.Children.Add(colorPicker);

        Content = containerGrid;
    }
}