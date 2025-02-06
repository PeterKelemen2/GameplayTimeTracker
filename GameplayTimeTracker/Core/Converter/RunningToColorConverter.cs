using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace GameplayTimeTracker;

public class RunningToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool runningState)
        {
            try
            {
                if (runningState)
                {
                    return new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Running"]));
                }
                else
                {
                    return new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"]));
                }
            }
            catch
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}