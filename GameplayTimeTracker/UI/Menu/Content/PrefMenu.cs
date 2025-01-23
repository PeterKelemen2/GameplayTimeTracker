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
        Binding swsBinding = new Binding("StartWithSystem") { Source = settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(pref1.checkBox, CheckBox.IsCheckedProperty, swsBinding);
        BindingHelper.SetColorBinding(pref1.textBlock, ForegroundProperty, settings, "Font");
        // BindingHelper.SetColorBinding(pref1.checkBox.boxBorder, Border.BorderBrushProperty, settings, "Font");
        // BindingHelper.SetColorBinding(pref1.checkBox.tickMark, Shape.FillProperty, settings, "Font");

        Panel.Children.Add(pref1);

        PrefEntry pref2 = new PrefEntry("Prefer SteamGridDB Images", settings.PreferSteamGridDBImage);
        Binding preferSGDBBinding = new Binding("PreferSteamGridDBImage")
            { Source = settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(pref2.checkBox, CheckBox.IsCheckedProperty, preferSGDBBinding);
        BindingHelper.SetColorBinding(pref2.textBlock, ForegroundProperty, settings, "Font");
        Panel.Children.Add(pref2);

        TextBlock sgdbApiKeyBlock =
            UIHelper.CreateTextBlock(text: "SteamGridDB API Key", margin: new Thickness(40, 0, 0, 0), isBold: false);
        TextBox sgdbApiKeyBox = UIHelper.CreateTextBox();
        Binding sgdbApiKeyBinding = new Binding("SGDBApiKey") { Source = settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(sgdbApiKeyBox, TextBox.TextProperty, sgdbApiKeyBinding);
        BindingHelper.SetColorBinding(sgdbApiKeyBlock, ForegroundProperty, settings, "Font");
        sgdbApiKeyBox.Margin = new Thickness(40, 0, 0, 0);
        Panel.Children.Add(sgdbApiKeyBlock);
        Panel.Children.Add(sgdbApiKeyBox);

        PrefEntry pref3 = new PrefEntry("Quick Add", settings.QuickAdd);
        Binding quickAddBinding = new Binding("QuickAdd") { Source = settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(pref3.checkBox, CheckBox.IsCheckedProperty, quickAddBinding);
        BindingHelper.SetColorBinding(pref3.textBlock, ForegroundProperty, settings, "Font");
        Panel.Children.Add(pref3);
    }
}