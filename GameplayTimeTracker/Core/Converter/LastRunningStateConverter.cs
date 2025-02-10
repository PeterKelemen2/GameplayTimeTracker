using System;
using System.Globalization;
using System.Windows.Data;

namespace GameplayTimeTracker;

public class LastRunningStateConverter : IValueConverter
{
    public object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
    {
        if (value is bool state) return state ? "Started: " : "Ended: ";

        return "Ended";
    }

    public object ConvertBack(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
    {
        Console.WriteLine("Converting back");
        return "aaaa";
    }
}