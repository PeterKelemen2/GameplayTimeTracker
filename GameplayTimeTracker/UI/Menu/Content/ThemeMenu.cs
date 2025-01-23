using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GameplayTimeTracker.Settings;

namespace GameplayTimeTracker.Menu.Content;

public class ThemeMenu : UserControl
{
    public StackPanel Panel = new StackPanel();

    public ThemeMenu(AppSettings settings)
    {
        Panel = new StackPanel();
        // PrefEntry pref1 = new PrefEntry("Pref", false);
        // Panel.Children.Add(pref1);

        StackPanel colorEntryPanel = new StackPanel();
        ScrollViewer colorEntryScrollViewer = new ScrollViewer
        {
            Height = 400,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
            Padding = new Thickness(5),
        };
        Console.WriteLine($"Themes count: {settings.ThemesList.Count}");
        foreach (var theme in settings.ThemesList)
        {
            foreach (var color in theme.Colors)
            {
                ColorEntry colorEntry =
                    new ColorEntry(color.Key, color.Value, AppColors.CardColor1, AppColors.CardColor2);
                colorEntryPanel.Children.Add(colorEntry);
            }
        }

        colorEntryScrollViewer.Content = colorEntryPanel;
        Panel.Children.Add(colorEntryScrollViewer);
    }
}