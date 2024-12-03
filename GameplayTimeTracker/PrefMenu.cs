using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GameplayTimeTracker;

public class PrefMenu : UserControl
{
    public Grid container { get; set; }
    public StackPanel Panel { get; set; }
    public TextBlock Label { get; set; }

    public PrefMenu(StackPanel stackPanel)
    {
        Panel = stackPanel;
        // CreateMenu();
    }

    public void CreateMenuMethod()
    {
        Panel.Children.Clear();
        Label = new TextBlock
        {
            Text = "Work in progress!\nYou can change settings in \nDocuments/Gameplay Time Tracker/settings.json",
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center,
            FontSize = 16,
            Foreground = new SolidColorBrush(Utils.FontColor),
        };
        Panel.Children.Add(Label);
    }

    public void CreateMenu(object sender, MouseButtonEventArgs e)
    {
        CreateMenuMethod();
    }
}