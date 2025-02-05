using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace GameplayTimeTracker;

public class TimeArrayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int[] timeArray && timeArray.Length == 3)
        {
            return $"{timeArray[0]}h {timeArray[1]}m {timeArray[2]}s";
        }

        return "0h 0m 0s";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string formattedString)
        {
            var match = Regex.Match(formattedString, @"(?:(\d+)[h\-:]?)?\s*(?:(\d+)[m\-:]?)?\s*(?:(\d+)[s]?)?");
            int h = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 0;
            int m = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;
            int s = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;

            m += s / 60;
            s %= 60;
            h += m / 60;
            m %= 60;

            return new[] { h, m, s };
        }

        return new int[] { 0, 0, 0 };
    }
}