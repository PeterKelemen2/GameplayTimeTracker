using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace GameplayTimeTracker;

public class RunningToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
    {
        if (value is bool isRunning)
        {
            return isRunning ? 1.0 : 0.7;
        }

        return 0.0;
    }

    public object ConvertBack(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
    {
        return 0.0;
    }
}