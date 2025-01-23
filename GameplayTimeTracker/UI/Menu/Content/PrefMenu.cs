using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using GameplayTimeTracker.Settings;

namespace GameplayTimeTracker.Menu.Content;

public class PrefMenu : UserControl
{
    public StackPanel Panel;

    public PrefMenu(AppSettings settings)
    {
        Panel = new StackPanel();
        PrefEntry pref1 = new PrefEntry("Start With System", settings.StartWithSystem);
        Binding swsBinding = new Binding("StartWithSystem")
        {
            Source = settings,
            Mode = BindingMode.TwoWay,
        };
        BindingOperations.SetBinding(pref1.checkBox, CheckBox.IsCheckedProperty, swsBinding);

        Panel.Children.Add(pref1);

        PrefEntry pref2 = new PrefEntry("Prefer SteamGridDB Images", settings.PreferSteamGridDBImage);
        Panel.Children.Add(pref2);

        TextBox sgdbApiKeyBox = UIHelper.CreateTextBox(placeholder: "API Key");
        sgdbApiKeyBox.Margin = new Thickness(40, 0, 0, 0);
        Panel.Children.Add(sgdbApiKeyBox);

        PrefEntry pref3 = new PrefEntry("Quick Add", settings.QuickAdd);
        Panel.Children.Add(pref3);
    }
}