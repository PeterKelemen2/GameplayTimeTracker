using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameplayTimeTracker.Menu.Content;

public class PrefMenu : UserControl
{
    public StackPanel Panel = new StackPanel();

    public PrefMenu()
    {
        Panel = new StackPanel();
        PrefEntry pref1 = new PrefEntry("Pref", false);
        Panel.Children.Add(pref1);
        Rectangle rect = new Rectangle
        {
            Width = 200,
            Height = 200,
            Fill = Brushes.Black,
        };
        Panel.Children.Add(rect);
    }
}