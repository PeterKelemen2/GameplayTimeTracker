using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Documents;
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
            Effect = AppEffects.DropShadowIcon,
            Margin = margin,
            // Margin = new Thickness(leftMargin, 0, 0, 5)
        };
        sample.Style = (Style)Application.Current.FindResource("RoundedTextBox");

        return sample;
    }

    public static TextBlock CreateTextBlock(string text = "", string description = "",
        HorizontalAlignment hA = HorizontalAlignment.Left, VerticalAlignment vA = VerticalAlignment.Top,
        double fontSize = Common.TextFontSize, Thickness margin = new(), bool isBold = true)
    {
        var sampleTextBlock = new TextBlock
        {
            Text = text,
            FontWeight = isBold ? FontWeights.Bold : FontWeights.Regular,
            FontSize = fontSize,
            Foreground =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
            HorizontalAlignment = hA,
            VerticalAlignment = vA,
            Margin = margin,
            Effect = AppEffects.DropOuterGlow,
        };
        TextOptions.SetTextRenderingMode(sampleTextBlock, TextRenderingMode.Aliased);

        if (description != "")
        {
            var descRun = new Run
            {
                Text = "\n" + description, FontSize = fontSize - 3,
                Foreground = new SolidColorBrush(ColorHelper.AdjustBrightness(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"]), 0.9)),
            };
            sampleTextBlock.Inlines.Add(descRun);
        }

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