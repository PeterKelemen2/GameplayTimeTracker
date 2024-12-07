using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameplayTimeTracker;

public class PrefEntry : UserControl
{
    private StackPanel ParentPanel;
    private Grid containerGrid;
    private TextBlock textBlock;
    public CheckBox checkBox { get; set; }

    public String PrefName { get; set; }
    public bool PrefValue { get; set; }
    private double padding = 15;

    public PrefEntry(StackPanel parent, string prefName, bool prefValue)
    {
        ParentPanel = parent;
        PrefName = prefName;
        PrefValue = prefValue;

        containerGrid = new Grid
        {
            Width = 300,
            Height = 50,
            Margin = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Left,
        };

        textBlock = new TextBlock
        {
            Text = PrefName,
            Foreground = new SolidColorBrush(Utils.FontColor),
            FontSize = 17,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(padding, 0, 0, 0),
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