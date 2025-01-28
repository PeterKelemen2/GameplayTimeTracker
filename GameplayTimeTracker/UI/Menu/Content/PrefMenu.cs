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

    public PrefMenu()
    {
        Panel = new StackPanel();
        PrefEntry pref1 = new PrefEntry("Start With System", Common.Settings.StartWithSystem);
        Binding swsBinding = new Binding("StartWithSystem") { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(pref1.toggleButton, CustomToggleButton.IsToggledProperty, swsBinding);
        BindingHelper.SetColorBinding(pref1.textBlock, ForegroundProperty, "Font");
        // BindingHelper.SetColorBinding(pref1.checkBox.boxBorder, Border.BorderBrushProperty, settings, "Font");
        // BindingHelper.SetColorBinding(pref1.checkBox.tickMark, Shape.FillProperty, settings, "Font");

        Panel.Children.Add(pref1);

        PrefEntry pref2 = new PrefEntry("Prefer SteamGridDB Images", Common.Settings.PreferSteamGridDBImage);
        Binding preferSGDBBinding = new Binding("PreferSteamGridDBImage")
            { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(pref2.toggleButton, CustomToggleButton.IsToggledProperty, preferSGDBBinding);
        BindingHelper.SetColorBinding(pref2.textBlock, ForegroundProperty, "Font");
        Panel.Children.Add(pref2);

        TextBlock sgdbApiKeyBlock =
            UIHelper.CreateTextBlock(text: "SteamGridDB API Key", margin: new Thickness(40, 0, 0, 0), isBold: false);
        TextBox sgdbApiKeyBox = UIHelper.CreateTextBox();
        Binding sgdbApiKeyBinding = new Binding("SGDBApiKey") { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(sgdbApiKeyBox, TextBox.TextProperty, sgdbApiKeyBinding);
        BindingHelper.SetColorBinding(sgdbApiKeyBlock, ForegroundProperty, "Font");
        sgdbApiKeyBox.Margin = new Thickness(40, 0, 0, 0);
        Panel.Children.Add(sgdbApiKeyBlock);
        Panel.Children.Add(sgdbApiKeyBox);

        PrefEntry pref3 = new PrefEntry("Quick Add", Common.Settings.QuickAdd);
        Binding quickAddBinding = new Binding("QuickAdd") { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(pref3.toggleButton, CustomToggleButton.IsToggledProperty, quickAddBinding);
        BindingHelper.SetColorBinding(pref3.textBlock, ForegroundProperty, "Font");
        Panel.Children.Add(pref3);


        Grid displayGrid = new Grid { Width = 380, Margin = new Thickness(25, 10, 25, 10) };
        
        TextBlock DisplayTypeBlock = UIHelper.CreateTextBlock(text: "Display", isBold: false, fontSize: 17);
        DisplayTypeBlock.Effect = AppEffects.dropShadowText;
        DisplayTypeBlock.HorizontalAlignment = HorizontalAlignment.Left;
        displayGrid.Children.Add(DisplayTypeBlock);
        
        ComboBox DisplayTypeComboBox = new ComboBox
        {
            Width = 120,
            Height = 30,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            ItemsSource = Enum.GetValues(typeof(GameDisplay)),
            SelectedItem = Common.Settings.Display
        };
        displayGrid.Children.Add(DisplayTypeComboBox);
        Panel.Children.Add(displayGrid);
        DisplayTypeComboBox.SelectionChanged += (s, e) =>
        {
            if (DisplayTypeComboBox.SelectedItem is GameDisplay selectedValue)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                Common.Settings.Display = selectedValue;
        
                AppAnimations.FadeOutMainPanel.Completed += OnFadeOutCompleted;
                mainWindow.MainPanel.BeginAnimation(OpacityProperty, AppAnimations.FadeOutMainPanel);
            }
        };
        
        Grid frequencyGrid = new Grid { Width = 380, Margin = new Thickness(25, 10, 25, 20) };

        TextBlock SaveFrequencyBlock = UIHelper.CreateTextBlock(text: "Save Frequency", isBold: false, fontSize: 17);
        SaveFrequencyBlock.Effect = AppEffects.dropShadowText;
        SaveFrequencyBlock.HorizontalAlignment = HorizontalAlignment.Left;
        frequencyGrid.Children.Add(SaveFrequencyBlock);
        ComboBox SaveFrequencyComboBox = new ComboBox
        {
            Width = 120,
            Height = 30,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            ItemsSource = Common.saveFreqArray,
            SelectedItem = Common.Settings.SavingFrequencyInMinutes
        };

        SaveFrequencyComboBox.SelectionChanged += (s, e) =>
        {
            Common.Settings.SavingFrequencyInMinutes = (int)SaveFrequencyComboBox.SelectedItem;
        };

        frequencyGrid.Children.Add(SaveFrequencyComboBox);
        Panel.Children.Add(frequencyGrid);
    }

    private void OnFadeOutCompleted(object sender, EventArgs e)
    {
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        mainWindow.ShowCards();
        mainWindow.MainPanel.BeginAnimation(OpacityProperty, AppAnimations.FadeInMainPanel);
        AppAnimations.FadeOut.Completed -= OnFadeOutCompleted;
    }
}