using System;
using System.Globalization;
using System.Windows.Data;
using GameplayTimeTracker.Extensions;

namespace GameplayTimeTracker.Converters;

public class TimeSpanPrettyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan ts)
            return ts.GetPretty();

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}