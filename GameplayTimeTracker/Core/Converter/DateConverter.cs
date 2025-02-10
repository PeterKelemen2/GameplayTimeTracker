using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace GameplayTimeTracker;

public class DateConverter : IValueConverter
{
    public object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
    {
        if (value is DateTime date && date.Year > 1000)
        {
            DateTime today = DateTime.Now.Date;
            DateTime yesterday = today.AddDays(-1);

            if (date.Date == today)
                return $"Today, {date:HH:mm}";
            if (date.Date == yesterday)
                return $"Yesterday, {date:HH:mm}";
            else
                return $"{date:yyyy.MM.dd HH:mm}";
        }

        return "Never";
    }

    public object ConvertBack(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
    {
        Console.WriteLine("Converting back");
        return DateTime.Parse((string)value);
    }
}