using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GameplayTimeTracker;

public class HorizontalGradientBrushConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is string color1 && values[1] is string color2)
        {
            try
            {
                Color startColor = (Color)ColorConverter.ConvertFromString(color1);
                Color endColor = (Color)ColorConverter.ConvertFromString(color2);

                return new LinearGradientBrush(startColor, endColor, new Point(0, 0), new Point(1, 0));
            }
            catch
            {
                return Brushes.Transparent; // Fallback in case of conversion failure
            }
        }
        return Brushes.Transparent; // Default fallback
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
