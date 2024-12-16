using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GameplayTimeTracker;

public class PrefMenu : UserControl
{
    public StackPanel Panel { get; set; }

    public Settings CurrentSettings { get; set; }

    public Dictionary<string, bool> Prefs { get; set; }
    public bool StartWithSystem { get; set; }
    public Action<bool, bool> TileGradUpdateMethod;
    public Action<bool> TileBgImagesMethod;
    public Action UpdateLegacyMethod;

    public PrefMenu(StackPanel stackPanel, Settings settings, Action<bool, bool> tileGradUpdateMethod,
        Action<bool> tileBgImagesMethod, Action updateLegacyMethod = null)
    {
        Panel = stackPanel;
        CurrentSettings = settings;
        TileGradUpdateMethod = tileGradUpdateMethod;
        TileBgImagesMethod = tileBgImagesMethod;
        UpdateLegacyMethod = updateLegacyMethod;
        // CreateMenu();
        Prefs = new Dictionary<string, bool>();
        Prefs.Add("Start with system", CurrentSettings.StartWithSystem);
        Prefs.Add("Horizontal Tile Gradient", CurrentSettings.HorizontalTileGradient);
        Prefs.Add("Horizontal Edit Gradient", CurrentSettings.HorizontalEditGradient);
        Prefs.Add("Bigger Background Images", CurrentSettings.BigBgImages);
    }

    public void CreateMenuMethod()
    {
        Panel.Children.Clear();
        Panel.Children.Add(GetNewEntry("Start with system", CurrentSettings.StartWithSystem));
        Panel.Children.Add(GetNewEntry("Horizontal Tile Gradient", CurrentSettings.HorizontalTileGradient));
        Panel.Children.Add(GetNewEntry("Horizontal Edit Gradient", CurrentSettings.HorizontalEditGradient));

        PrefEntry newEntry = new PrefEntry(Panel, "Bigger Background Images", CurrentSettings.BigBgImages);
        newEntry.checkBox.Checked += (sender, e) => UpdateBgImageSize(true);
        newEntry.checkBox.Unchecked += (sender, e) => UpdateBgImageSize(false);
        Panel.Children.Add(newEntry);

        CustomButton updateLegacyData =
            new CustomButton(text: "Update legacy data", width: 150, height: 40, type: ButtonType.Positive);
        // updateLegacyData.HorizontalAlignment = HorizontalAlignment.Center;
        // updateLegacyData.VerticalAlignment = VerticalAlignment.Center;
        updateLegacyData.Click += Update_Click;
        Panel.Children.Add(updateLegacyData);
        // EditButton.Margin = new Thickness(0, topMargin, 100, 0);
        // EditButton.Click += ToggleEdit_Click;
        

        DoubleAnimation fadeInAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Panel.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        // Panel.Children.Add(GetNewEntry("Bigger Background Images", CurrentSettings.BigBgImages));
    }


    private PrefEntry GetNewEntry(String prefName, bool value)
    {
        PrefEntry newEntry = new PrefEntry(Panel, prefName, value);
        newEntry.checkBox.Checked += (sender, e) => UpdatePrefs(newEntry.PrefName, true);
        newEntry.checkBox.Unchecked += (sender, e) => UpdatePrefs(newEntry.PrefName, false);

        return newEntry;
    }

    private void UpdatePrefs(String key, bool value)
    {
        if (Prefs.ContainsKey(key)) Prefs[key] = value;
        TileGradUpdateMethod(Prefs["Horizontal Tile Gradient"], Prefs["Horizontal Edit Gradient"]);
        SaveToFile();
    }

    private void UpdateBgImageSize(bool value)
    {
        if (Prefs.ContainsKey("Bigger Background Images")) Prefs["Bigger Background Images"] = value;
        TileBgImagesMethod(value);
        SaveToFile();
    }

    private void SaveToFile()
    {
        JsonHandler jsonHandler = new JsonHandler();
        jsonHandler.WritePrefsToFile(Prefs["Start with system"], Prefs["Horizontal Tile Gradient"],
            Prefs["Horizontal Edit Gradient"], Prefs["Bigger Background Images"]);
    }

    public void CreateMenu(object sender, MouseButtonEventArgs e)
    {
        CreateMenuMethod();
    }
    
    public void Update_Click(object sender, RoutedEventArgs e)
    {
        if (UpdateLegacyMethod != null)
        {
            UpdateLegacyMethod();
        }
    }
}