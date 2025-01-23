using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameplayTimeTracker.UI.Menu.Content;

public class CustomCheckBox : UserControl
{
    public StackPanel StackPanel { get; set; }
    public Border boxBorder;
    public CheckBox customCheckBox;
    public Rectangle tickMark;

    public CustomCheckBox()
    {
        customCheckBox = new CheckBox();

        StackPanel = new StackPanel();
        boxBorder = new Border
        {
            Width = 20,
            Height = 20,
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(4),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        tickMark = new Rectangle
        {
            Width = 10,
            Height = 10,
            Fill = Brushes.Transparent
        };

        boxBorder.Child = tickMark;

        StackPanel.Children.Add(boxBorder);
        // StackPanel.Children.Add(tickMark);
        // stackPanel.Children.Add(new ContentPresenter { VerticalAlignment = VerticalAlignment.Center });

        // Set the content of the CheckBox to the custom StackPanel
        customCheckBox.Content = StackPanel;

        customCheckBox.Checked += (s, e) => tickMark.Fill = Brushes.White;
        customCheckBox.Unchecked += (s, e) => tickMark.Fill = Brushes.Transparent;

        // Content = StackPanel;
    }
}