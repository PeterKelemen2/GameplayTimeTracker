using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GameplayTimeTracker;

public class PrefEntry : UserControl
{
    private StackPanel ParentPanel;
    private Grid containerGrid;
    public TextBlock textBlock;
    public CustomToggleButton toggleButton { get; set; }
    public String PrefName { get; set; }
    public bool PrefValue { get; set; }
    private double padding = 15;

    public PrefEntry(string prefName, bool prefValue, double width = 380, string description = "")
    {
        PrefName = prefName;
        PrefValue = prefValue;

        containerGrid = new Grid
        {
            Width = width,
            Height = 50,
            Margin = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        textBlock = new TextBlock
        {
            Text = PrefName,
            Foreground =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
            FontSize = 17,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(padding, 0, 0, 0),
            Effect = AppEffects.dropShadowText,
        };
        if (description != "")
        {
            var descRun = new Run
            {
                Text = "\n" + description, FontSize = textBlock.FontSize - 3,
                Foreground = new SolidColorBrush(ColorHelper.AdjustBrightness(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"]), 0.9)),
            };
            textBlock.Inlines.Add(descRun);
        }
        containerGrid.Children.Add(textBlock);

        toggleButton = new CustomToggleButton();
        toggleButton.HorizontalAlignment = HorizontalAlignment.Right;
        toggleButton.ButtonGrid.Margin = new Thickness(0, 0, padding, 0);
        containerGrid.Children.Add(toggleButton);

        Content = containerGrid;
    }
}