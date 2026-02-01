using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GameplayTimeTracker.Converters;

public class HexToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string hex)
        {
            try
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom(hex));
            }
            catch
            {
                return Brushes.Transparent;
            }
        }

        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}