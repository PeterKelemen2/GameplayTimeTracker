using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GameplayTimeTracker.Converters;

public class ColorBrightnessConverter : IValueConverter
{
    public double Factor { get; set; } = 0.2; // Positive -> lighten, Negative -> darken

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            var color = brush.Color;

            if (Factor >= 0)
            {
                // Lighten - blend with white
                byte Lighten(byte channel) => (byte)(channel + (255 - channel) * Factor);
                
                return new SolidColorBrush(Color.FromArgb(
                    color.A,
                    Lighten(color.R),
                    Lighten(color.G),
                    Lighten(color.B)));
            }
            else
            {
                // Darken - multiply by factor
                byte Darken(byte channel) => (byte)(channel * (1 + Factor));
                
                return new SolidColorBrush(Color.FromArgb(
                    color.A,
                    Darken(color.R),
                    Darken(color.G),
                    Darken(color.B)));
            }
        }
        
        // Fallback to original value or transparent
        return value ?? Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}