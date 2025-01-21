using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace GameplayTimeTracker;

public static class UIHelper
{
    public static TextBox CreateTextBox(string text = "", HorizontalAlignment hA = HorizontalAlignment.Left,
        VerticalAlignment vA = VerticalAlignment.Bottom, double width = 180, Thickness margin = new())
    {
        TextBox sample = new TextBox
        {
            Text = text,
            Width = width,
            Height = Common.TextBoxHeight,
            HorizontalAlignment = hA,
            VerticalAlignment = vA,
            TextAlignment = TextAlignment.Left,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Center,
            Effect = AppEffects.dropShadowIcon,
            Margin = margin,
            // Margin = new Thickness(leftMargin, 0, 0, 5)
        };
        sample.Style = (Style)Application.Current.FindResource("RoundedTextBox");

        return sample;
    }

    public static TextBlock CreateTextBlock(string text = "", HorizontalAlignment hA = HorizontalAlignment.Left,
        VerticalAlignment vA = VerticalAlignment.Top, double fontSize = Common.TextFontSize, Thickness margin = new(),
        bool isBold = true)
    {
        var sampleTextBlock = new TextBlock
        {
            Text = text,
            FontWeight = isBold ? FontWeights.Bold : FontWeights.Regular,
            FontSize = fontSize,
            Foreground = new SolidColorBrush(AppColors.Font),
            HorizontalAlignment = hA,
            VerticalAlignment = vA,
            Margin = margin,
            // Margin = new Thickness(leftMargin + 5, 5, 0, 5),
            Effect = AppEffects.dropShadowText,
        };

        return sampleTextBlock;
    }
}