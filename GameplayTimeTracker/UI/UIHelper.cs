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
using GameplayTimeTracker.Settings;

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

    public static CustomButton CreateBrowseButtonRB(double w, double h, double m)
    {
        CustomButton button = new CustomButton(w: w, h: h, bRad: 3,
            hA: HorizontalAlignment.Right, vA: VerticalAlignment.Bottom, bImgPath: AppFiles.FolderIcon);
        button.Margin = new Thickness(0, 0, m, m);
        return button;
    }

    public static Grid CreateAddEntryGrid(AppSettings settings, string text, Thickness textMargin, string boxText = "",
        double boxWidth = 220,
        string binding = "", Entry entry = null)
    {
        Grid grid = new Grid { HorizontalAlignment = HorizontalAlignment.Center };
        TextBlock textBlock = CreateTextBlock(text, margin: textMargin);
        BindingHelper.SetColorBinding(textBlock, TextBlock.ForegroundProperty, "Font");

        TextBox textBox = CreateTextBox(boxText, width: boxWidth);
        if (binding != "" && entry != null)
        {
            Binding exeBinding = new Binding(binding) { Source = entry, Mode = BindingMode.TwoWay, };
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, exeBinding);
        }

        grid.Children.Add(textBlock);
        grid.Children.Add(textBox);
        return grid;
    }
}