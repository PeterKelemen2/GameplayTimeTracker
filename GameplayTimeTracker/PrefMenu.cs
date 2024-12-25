using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public Action<SettingsMenu> RestoreBackupMethod;
    public Action ShowTilesMethod;
    SettingsMenu settingsMenu;
    private CustomButton createBackupButton;
    private CustomButton restoreBackupButton;


    public PrefMenu(StackPanel stackPanel, Settings settings, Action<bool, bool> tileGradUpdateMethod,
        Action<bool> tileBgImagesMethod, Action<SettingsMenu> restoreBackupMethod = null, Action showTiles = null, SettingsMenu sMenu = null)
    {
        Panel = stackPanel;
        CurrentSettings = settings;
        TileGradUpdateMethod = tileGradUpdateMethod;
        TileBgImagesMethod = tileBgImagesMethod;
        RestoreBackupMethod = restoreBackupMethod;
        ShowTilesMethod = showTiles;
        settingsMenu = sMenu;
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

        createBackupButton = new CustomButton(text: "Create Backup data", width: 170, height: 40,
            type: ButtonType.Positive,
            isBold: true);
        createBackupButton.Margin = new Thickness(0,30,0,0);
        createBackupButton.Click += CreateBackup_Click;
        Panel.Children.Add(createBackupButton);

        restoreBackupButton =
            new CustomButton(text: "Restore Backup data", width: 170, height: 40, type: ButtonType.Default,
                isBold: true);
        restoreBackupButton.Margin = new Thickness(10);
        restoreBackupButton.Click += Restore_Click;
        Panel.Children.Add(restoreBackupButton);


        DoubleAnimation fadeInAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Panel.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
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

    public void Restore_Click(object sender, RoutedEventArgs e)
    {
        if (RestoreBackupMethod != null)
        {
            RestoreBackupMethod(settingsMenu);
            // PopupMenu popup = new PopupMenu(text: "Please restart application for this to take effect",
            //     type: PopupType.OK);
            // settingsMenu.CloseMenuMethod();
            ShowTilesMethod();
            // popup.OpenMenu();
        }
    }

    public void CreateBackup_Click(object sender, RoutedEventArgs e)
    {
        JsonHandler handler = new JsonHandler();
        bool created = handler.BackupDataFile();
        PopupMenu popup = new PopupMenu();
        if (created)
        {
            popup = new PopupMenu(text: "Backup created successfully!",
                type: PopupType.OK);
            restoreBackupButton.Enable();
        }
        else
        {
            popup = new PopupMenu(text: "Could not create backup!",
                type: PopupType.OK);
        }

        settingsMenu.CloseMenuMethod();
        popup.OpenMenu();
    }
}