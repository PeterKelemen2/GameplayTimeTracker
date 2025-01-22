using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameplayTimeTracker.Menu.Content;

public class ThemeMenu : UserControl
{
    public StackPanel Panel = new StackPanel();

    public ThemeMenu()
    {
        Panel = new StackPanel();
        PrefEntry pref1 = new PrefEntry("Pref", false);
        Panel.Children.Add(pref1);
        Rectangle rect = new Rectangle
        {
            Width = 200,
            Height = 200,
            Fill = Brushes.White,
        };
        Panel.Children.Add(rect);
    }
}