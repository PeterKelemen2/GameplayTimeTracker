using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GameplayTimeTracker.Menu;

public class EditMenu : CustomMenu
{
    private Entry Entry;
    private StackPanel StackPanel;

    public EditMenu(Entry entry, double width = 300, double height = 400) : base(width, height)
    {
        Entry = entry;

        StackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
        };
        MenuContentGrid.Children.Add(StackPanel);

        TextBlock title = new TextBlock
        {
            Text = $"Editing ",
            FontSize = Common.EditTitleFontSize,
            FontWeight = FontWeights.Regular,
            Foreground = new SolidColorBrush(AppColors.Font),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 10, 0, 0),
        };
        title.Inlines.Add(new Run{Text = Entry.Name, FontWeight = FontWeights.Bold});
        StackPanel.Children.Add(title);
    }
}