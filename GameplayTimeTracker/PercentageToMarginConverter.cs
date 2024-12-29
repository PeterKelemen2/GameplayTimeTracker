using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GameplayTimeTracker;

public class PercentageToMarginConverter : IValueConverter
{
    private readonly double _percentage;

    public PercentageToMarginConverter(double percentage)
    {
        _percentage = percentage;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Ensure the value is a valid window width
        if (value is double width)
        {
            // Calculate the margin as a percentage of the width
            return new Thickness(width * _percentage / 100, 0, 0, 0);  // Horizontal only
        }

        return new Thickness(0);  // Default to 0 margin if the value is invalid
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();  // No need for reverse conversion
    }
}