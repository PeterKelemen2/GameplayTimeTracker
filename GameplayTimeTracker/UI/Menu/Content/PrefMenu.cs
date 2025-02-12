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
        CreatePref("Start With System", "", "StartWithSystem", Common.Settings.StartWithSystem);

        CreatePref("Prefer SteamGridDB Images", "Uses local icon image if disabled", "PreferSteamGridDBImage",
            Common.Settings.PreferSteamGridDBImage);

        Grid apiKeyGrid = new Grid { Margin = new Thickness(40, 10, 25, 10) };

        TextBlock sgdbApiKeyBlock =
            UIHelper.CreateTextBlock(text: "SGDB API Key", margin: new Thickness(0, 0, 0, 0), isBold: false,
                vA: VerticalAlignment.Center);
        SetSGDBClickableText(sgdbApiKeyBlock);
        TextBox sgdbApiKeyBox = UIHelper.CreateTextBox(width: 220);
        Binding sgdbApiKeyBinding = new Binding("SGDBApiKey")
        {
            Source = Common.Settings, Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            // Converter = new ApiKeyTextConverter()
        };
        BindingOperations.SetBinding(sgdbApiKeyBox, TextBox.TextProperty, sgdbApiKeyBinding);
        BindingHelper.SetColorBinding(sgdbApiKeyBlock, ForegroundProperty, "Font");
        sgdbApiKeyBox.Margin = new Thickness(95, 0, 0, 0);
        sgdbApiKeyBox.Text = new ApiKeyTextConverter().Convert(Common.Settings.SGDBApiKey) as string;
        sgdbApiKeyBox.GotFocus += (s, e) =>
        {
            if (sgdbApiKeyBox.Text.Equals("No API key set.")) sgdbApiKeyBox.Text = "";
        };
        sgdbApiKeyBox.LostFocus += (s, e) =>
        {
            if (sgdbApiKeyBox.Text.Equals("")) sgdbApiKeyBox.Text = "No API key set.";
        };
        apiKeyGrid.Children.Add(sgdbApiKeyBlock);
        apiKeyGrid.Children.Add(sgdbApiKeyBox);
        _stackPanel.Children.Add(apiKeyGrid);

        CreatePref("Quick Add", "No edit menu when adding", "QuickAdd", Common.Settings.QuickAdd);

        CreatePref("Performance Mode", "More modest animations", "PerformanceMode", Common.Settings.PerformanceMode);

        CreatePref("Backup On Exit", "Application Data File", "BackupOnExit", Common.Settings.BackupOnExit);

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

    private void CreatePref(string name, string desc, string bindPath, bool value)
    {
        PrefEntry pref = new PrefEntry(name, value, description: desc);
        Binding swsBinding = new Binding(bindPath) { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(pref.toggleButton, CustomToggleButton.IsToggledProperty, swsBinding);
        BindingHelper.SetColorBinding(pref.textBlock, ForegroundProperty, "Font");
        _stackPanel.Children.Add(pref);
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