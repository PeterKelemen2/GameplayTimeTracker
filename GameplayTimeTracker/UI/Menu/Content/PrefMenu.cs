using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using GameplayTimeTracker.UI.Menu.Content;

namespace GameplayTimeTracker.Menu.Content;

public class PrefMenu : MenuContent
{
    public PrefMenu()
    {
        PrefEntry pref1 = new PrefEntry("Start With System", Common.Settings.StartWithSystem);
        Binding swsBinding = new Binding("StartWithSystem") { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(pref1.toggleButton, CustomToggleButton.IsToggledProperty, swsBinding);
        BindingHelper.SetColorBinding(pref1.textBlock, ForegroundProperty, "Font");
        _stackPanel.Children.Add(pref1);

        PrefEntry pref2 = new PrefEntry("Prefer SteamGridDB Images", Common.Settings.PreferSteamGridDBImage,
            description: "Uses local icon image if disabled");
        Binding preferSGDBBinding = new Binding("PreferSteamGridDBImage")
            { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(pref2.toggleButton, CustomToggleButton.IsToggledProperty, preferSGDBBinding);
        BindingHelper.SetColorBinding(pref2.textBlock, ForegroundProperty, "Font");
        _stackPanel.Children.Add(pref2);

        Grid apiKeyGrid = new Grid { Margin = new Thickness(40, 10, 25, 10) };

        TextBlock sgdbApiKeyBlock =
            UIHelper.CreateTextBlock(text: "SGDB API Key", margin: new Thickness(0, 0, 0, 0), isBold: false,
                vA: VerticalAlignment.Center);
        SetSGDBClickableText(sgdbApiKeyBlock);
        TextBox sgdbApiKeyBox = UIHelper.CreateTextBox();
        Binding sgdbApiKeyBinding = new Binding("SGDBApiKey") { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(sgdbApiKeyBox, TextBox.TextProperty, sgdbApiKeyBinding);
        BindingHelper.SetColorBinding(sgdbApiKeyBlock, ForegroundProperty, "Font");
        sgdbApiKeyBox.Margin = new Thickness(95, 0, 0, 0);
        apiKeyGrid.Children.Add(sgdbApiKeyBlock);
        apiKeyGrid.Children.Add(sgdbApiKeyBox);
        _stackPanel.Children.Add(apiKeyGrid);

        PrefEntry pref3 = new PrefEntry("Quick Add", Common.Settings.QuickAdd, description: "No edit menu when adding");
        Binding quickAddBinding = new Binding("QuickAdd") { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(pref3.toggleButton, CustomToggleButton.IsToggledProperty, quickAddBinding);
        BindingHelper.SetColorBinding(pref3.textBlock, ForegroundProperty, "Font");
        _stackPanel.Children.Add(pref3);

        PrefEntry performancePref = new PrefEntry("Performance Mode", Common.Settings.StartWithSystem,
            description: "More modest animations");
        Binding performanceBinding = new Binding("PerformanceMode")
            { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(performancePref.toggleButton, CustomToggleButton.IsToggledProperty,
            performanceBinding);
        BindingHelper.SetColorBinding(performancePref.textBlock, ForegroundProperty, "Font");
        _stackPanel.Children.Add(performancePref);

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
        _stackPanel.Children.Add(displayGrid);
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

        TextBlock SaveFrequencyBlock = UIHelper.CreateTextBlock(text: "Save Frequency", description: "In minutes",
            isBold: false, fontSize: 17);
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
        _stackPanel.Children.Add(frequencyGrid);
    }

    private void OnFadeOutCompleted(object sender, EventArgs e)
    {
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        mainWindow.ShowCards();
        mainWindow.MainPanel.BeginAnimation(OpacityProperty, AppAnimations.FadeInMainPanel);
        AppAnimations.FadeOut.Completed -= OnFadeOutCompleted;
    }

    private void SetSGDBClickableText(TextBlock textBlock)
    {
        textBlock.MouseDown += (s, e) =>
        {
            string url = "https://www.steamgriddb.com/profile/preferences/api";
            try
            {
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open link: {ex.Message}");
            }
        };

        textBlock.MouseEnter += (sender, args) =>
        {
            textBlock.Foreground = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"]), 0.9));
        };

        textBlock.MouseLeave += (sender, args) =>
        {
            textBlock.Foreground = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"]), 1.0));
        };
    }
}