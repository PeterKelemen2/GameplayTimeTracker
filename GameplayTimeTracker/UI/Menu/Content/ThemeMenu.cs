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

        foreach (var theme in settings.ThemesList)
        {
            foreach (var color in theme.Colors)
            {
                ColorEntry colorEntry =
                    new ColorEntry(color.Key, color.Value, AppColors.CardColor1, AppColors.CardColor2);
                Panel.Children.Add(colorEntry);
            }
        }
    }
}