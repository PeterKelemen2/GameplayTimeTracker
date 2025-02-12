using System;
using System.Globalization;
using System.Windows.Data;

namespace GameplayTimeTracker;

public class ApiKeyTextConverter : IValueConverter
{
    public string noKeySet = "No API key set.";

    public object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
    {
        if (value is string apiKey) return string.IsNullOrEmpty(apiKey) ? noKeySet : apiKey;

        return "";
    }

    public object ConvertBack(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
    {
        if (value is string input)
        {
            if (string.Equals(input, noKeySet)) return "";
        
            return input;
        }

        return "";
    }
}