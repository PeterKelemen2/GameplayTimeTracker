using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class PrefEntry : UserControl
{
    private Grid containerGrid;
    private TextBlock textBlock;
    public CheckBox checkBox { get; set; }

    public String PrefName { get; set; }
    public bool PrefValue { get; set; }

    public PrefEntry(string prefName, bool prefValue)
    {
        PrefName = prefName;
        PrefValue = prefValue;

        containerGrid = new Grid
        {
            Width = 200,
            Height = 50,
            Margin = new Thickness(5),
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        textBlock = new TextBlock
        {
            Text = PrefName,
            Foreground = new SolidColorBrush(Utils.FontColor),
            FontSize = 17,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(15, 0, 0, 0),
        };
        containerGrid.Children.Add(textBlock);

        checkBox = new CheckBox
        {
            IsChecked = PrefValue,
            // Style = (Style)Application.Current.FindResource("CustomCheckBoxTemplate"),
            FontSize = 17,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, 15, 0),
        };
        checkBox.Template = (ControlTemplate)Application.Current.Resources["CustomCheckBoxTemplate"];

        containerGrid.Children.Add(checkBox);

        Content = containerGrid;
    }
}