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
    private TextBlock valueBlock;
    private Rectangle picker;
    private Rectangle pickerBg;
    private Rectangle bg;
    private Grid containerGrid;
    private ColorPicker colorPicker;
    private int fontSize = 15;

    public ColorEntry(string colorName, string colorValue, double width = 250)
    {
        ColorName = colorName;
        ColorValue = colorValue;

        containerGrid = new Grid
        {
            Width = width,
            Height = 70,
            Margin = new Thickness(15, 5, 5, 5),
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        bg = new Rectangle
        {
            Fill = new SolidColorBrush(Colors.Gray),
            Width = containerGrid.Width,
            Height = containerGrid.Height,
            RadiusX = 10,
            RadiusY = 10,
        };
        containerGrid.Children.Add(bg);

        nameBlock = new TextBlock
        {
            Text = colorName,
            FontSize = fontSize,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Colors.White),
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(10, 10, 0, 0),
        };
        containerGrid.Children.Add(nameBlock);

        valueBlock = new TextBlock
        {
            Text = colorValue,
            FontSize = fontSize,
            Foreground = new SolidColorBrush(Colors.White),
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(10, 0, 0, 10),
        };
        containerGrid.Children.Add(valueBlock);

        pickerBg = new Rectangle
        {
            Width = 30,
            Height = 30,
            Fill = new SolidColorBrush(Colors.White),
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, 10, 0),
        };
        containerGrid.Children.Add(pickerBg);

        colorPicker = new ColorPicker();
        colorPicker.Width = 30;
        colorPicker.Height = 30;
        colorPicker.ShowDropDownButton = false;
        colorPicker.HorizontalAlignment = HorizontalAlignment.Right;
        colorPicker.Margin = new Thickness(0, 0, 10, 0);
        colorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(colorValue);
        colorPicker.UsingAlphaChannel = false;
        colorPicker.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorValue));
        colorPicker.BorderThickness = new Thickness(0);
        colorPicker.BorderBrush = Brushes.Transparent;
        colorPicker.SelectedColorChanged += ColorPicker_SelectedColorChanged;
        containerGrid.Children.Add(colorPicker);

        Content = containerGrid;
    }

    private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
    {
        var selectedColor = e.NewValue;

        // Check if the selected color is not null
        if (selectedColor.HasValue)
        {
            // Get the selected color
            Color color = selectedColor.Value;

            colorPicker.Background = new SolidColorBrush(color);
            valueBlock.Text = color.ToString();
            // Print the color components (RGB and check for transparency)
            Console.WriteLine($"Selected Color: {valueBlock.Text}");
        }
        else
        {
            // Handle the case where no color was selected (optional)
            colorPicker.Background = new SolidColorBrush(Colors.Transparent); // Default color
            Console.WriteLine("No color selected.");
        }
    }
}