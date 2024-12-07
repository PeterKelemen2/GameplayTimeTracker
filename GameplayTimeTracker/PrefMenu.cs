using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameplayTimeTracker;

public class PrefMenu : UserControl
{
    public StackPanel Panel { get; set; }

    public Settings CurrentSettings { get; set; }

    public Dictionary<string, bool> Prefs { get; set; }
    public bool StartWithSystem { get; set; }
    public Action<bool, bool> TileGradUpdateMethod;

    public PrefMenu(StackPanel stackPanel, Settings settings, Action<bool, bool> tileGradUpdateMethod)
    {
        Panel = stackPanel;
        CurrentSettings = settings;
        TileGradUpdateMethod = tileGradUpdateMethod;
        // CreateMenu();
        Prefs = new Dictionary<string, bool>();
        Prefs.Add("Start with system", CurrentSettings.StartWithSystem);
        Prefs.Add("Horizontal Tile Gradient", CurrentSettings.HorizontalTileGradient);
        Prefs.Add("Horizontal Edit Gradient", CurrentSettings.HorizontalEditGradient);
    }

    public void CreateMenuMethod()
    {
        Panel.Children.Clear();
        Panel.Children.Add(GetNewEntry("Start with system", CurrentSettings.StartWithSystem));
        Panel.Children.Add(GetNewEntry("Horizontal Tile Gradient", CurrentSettings.HorizontalTileGradient));
        Panel.Children.Add(GetNewEntry("Horizontal Edit Gradient", CurrentSettings.HorizontalEditGradient));
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
        SaveToFile(Prefs);
    }

    private void SaveToFile(Dictionary<String, bool> prefs)
    {
        JsonHandler jsonHandler = new JsonHandler();
        jsonHandler.WritePrefsToFile(Prefs["Start with system"], Prefs["Horizontal Tile Gradient"],
            Prefs["Horizontal Edit Gradient"]);
    }

    public void CreateMenu(object sender, MouseButtonEventArgs e)
    {
        CreateMenuMethod();
    }
}