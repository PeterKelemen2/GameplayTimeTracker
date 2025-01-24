using System;
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

        TextBlock DisplayTypeBlock =
            UIHelper.CreateTextBlock(text: "Display", margin: new Thickness(25, 0, 0, 0), isBold: false, fontSize: 17);
        DisplayTypeBlock.Effect = AppEffects.dropShadowText;
        Panel.Children.Add(DisplayTypeBlock);
        ComboBox DisplayTypeComboBox = new ComboBox
        {
            Width = 150,
            Height = 30,
            Margin = new Thickness(40, 10, 0, 10),
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
            ItemsSource = Enum.GetValues(typeof(GameDisplay)),
            SelectedItem = settings.Display
        };
        // DisplayTypeComboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding("Display")
        // {
        //     Source = settings,
        //     Mode = BindingMode.TwoWay
        // });
        DisplayTypeComboBox.SelectionChanged += (s, e) =>
        {
            if (DisplayTypeComboBox.SelectedItem is GameDisplay selectedValue)
            {
                settings.Display = selectedValue;
                ((MainWindow)Application.Current.MainWindow).ShowCards();
            }
        };

        Panel.Children.Add(DisplayTypeComboBox);
    }
}