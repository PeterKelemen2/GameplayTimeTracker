using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameplayTimeTracker.Menu;

public class SettingsMenu : CustomMenu
{
    StackPanel SettingsContentPanel = new();
    StackPanel HeaderPanel = new();

    public SettingsMenu(double width = 400, bool performanceMode = true) : base(width, performanceMode)
    {
        HeaderPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        var blockMargin = new Thickness(10, 10, 10, 10);
        var PrefBlock = UIHelper.CreateTextBlock("Preferences", margin: blockMargin);
        var Themes = UIHelper.CreateTextBlock("Themes", margin: blockMargin);
        var SteamGridDB = UIHelper.CreateTextBlock("SteamGridDB", margin: blockMargin);
        var Options = UIHelper.CreateTextBlock("Options", margin: blockMargin);
        HeaderPanel.Children.Add(PrefBlock);
        HeaderPanel.Children.Add(Themes);
        HeaderPanel.Children.Add(SteamGridDB);
        HeaderPanel.Children.Add(Options);
        Border headerBorder = new Border
        {
            BorderThickness = new Thickness(0, 0, 0, 1),
            BorderBrush = Brushes.Gray,
            Child = HeaderPanel,
        };
        MenuContentPanel.Children.Add(headerBorder);

        SettingsContentPanel = new StackPanel();
        MenuContentPanel.Children.Add(SettingsContentPanel);

        PrefEntry pref1 = new PrefEntry(SettingsContentPanel, "Pref 1", false);
        SettingsContentPanel.Children.Add(pref1);
    }
}