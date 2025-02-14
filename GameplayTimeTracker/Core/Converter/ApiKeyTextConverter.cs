using System;
using System.Globalization;
using System.Windows.Data;

namespace GameplayTimeTracker;

public class ApiKeyTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
    {
        if (value is string apiKey) return string.IsNullOrEmpty(apiKey) ? Common.NoApiKeyText : apiKey;

        return "";
    }

    public object ConvertBack(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
    {
        if (value is string input)
        {
            if (string.Equals(input, Common.NoApiKeyText)) return "";

            return input;
        }

        return "";
    }
}