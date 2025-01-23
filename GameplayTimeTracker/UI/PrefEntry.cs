using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameplayTimeTracker;

public class PrefEntry : UserControl
{
    private StackPanel ParentPanel;
    private Grid containerGrid;
    public TextBlock textBlock;
    public CheckBox checkBox { get; set; }
    public String PrefName { get; set; }
    public bool PrefValue { get; set; }
    private double padding = 15;

    public PrefEntry(string prefName, bool prefValue, double width = 380)
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
            Foreground = new SolidColorBrush(AppColors.Font),
            FontSize = 17,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(padding, 0, 0, 0),
            Effect = AppEffects.dropShadowText,
        };
        containerGrid.Children.Add(textBlock);

        checkBox = new CheckBox
        {
            IsChecked = PrefValue,
            FontSize = 17,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, padding, 0),
        };
        checkBox.Template = (ControlTemplate)Application.Current.Resources["CustomCheckBoxTemplate"];

        containerGrid.Children.Add(checkBox);

        Content = containerGrid;
    }
}