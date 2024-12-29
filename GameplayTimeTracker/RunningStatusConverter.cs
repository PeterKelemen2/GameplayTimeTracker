using System;
using System.Globalization;
using System.Windows.Data;

namespace GameplayTimeTracker;

public class RunningStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Return "Running!" if IsRunning is true, otherwise return empty string
        return value is bool isRunning && isRunning ? "Running!" : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // No need for convert back, we only need to show the status
        throw new NotImplementedException();
    }
}
