using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker.Settings;

public class AppSettings : INotifyPropertyChanged
{
    private bool _startWithSystem = true;
    private string _sgdbApiKey = "";
    private bool _dontShowApiKeyPrompt = false;
    private bool _preferSGDBImages = true;
    private bool _quickAdd = false;
    private bool _performanceMode = false;
    private bool _backupOnExit = false;
    private bool _isRemoteSavingEnabled = false;
    private RemoteMachine _remoteMachine = new();

    private AppTheme _currentTheme;
    private GameDisplay _gameDisplay = GameDisplay.Vertical;
    private int _savingFreqInMin = 1;

    // private string _currentTheme = "Default";
    private List<AppTheme> _themesList = new();
    public event PropertyChangedEventHandler PropertyChanged;

    [JsonPropertyName("Start with System")]
    public bool StartWithSystem
    {
        get => _startWithSystem;
        set
        {
            SetField(ref _startWithSystem, value);
            DataHandler.ManageStartupShortcut(_startWithSystem);
        }
    }

    [JsonPropertyName("SteamGridDB API key")]
    public string SGDBApiKey
    {
        get => _sgdbApiKey;
        set { SetField(ref _sgdbApiKey, value); }
    }

    [JsonPropertyName("Dont Show API Key prompt")]
    public bool DontShowApiKeyPrompt
    {
        get => _dontShowApiKeyPrompt;
        set { SetField(ref _dontShowApiKeyPrompt, value); }
    }

    [JsonPropertyName("Prefer SteamGridDB Image")]
    public bool PreferSteamGridDBImage
    {
        get => _preferSGDBImages;
        set { SetField(ref _preferSGDBImages, value); }
    }

    [JsonPropertyName("Quick Add")]
    public bool QuickAdd
    {
        get => _quickAdd;
        set { SetField(ref _quickAdd, value); }
    }

    [JsonPropertyName("Performance Mode")]
    public bool PerformanceMode
    {
        get => _performanceMode;
        set { SetField(ref _performanceMode, value); }
    }

    [JsonPropertyName("Backup On Exit")]
    public bool BackupOnExit
    {
        get => _backupOnExit;
        set { SetField(ref _backupOnExit, value); }
    }


    [JsonPropertyName("Display Type")]
    public GameDisplay Display
    {
        get => _gameDisplay;
        set { SetField(ref _gameDisplay, value); }
    }

    [JsonPropertyName("Saving Frequency")]
    public int SavingFrequencyInMinutes
    {
        get => _savingFreqInMin;
        set { SetField(ref _savingFreqInMin, value); }
    }

    [JsonPropertyName("Remote Saving Enabled")]
    public bool IsRemoteSavingEnabled
    {
        get => _isRemoteSavingEnabled;
        set { SetField(ref _isRemoteSavingEnabled, value); }
    }

    [JsonPropertyName("Remote Machine")]
    public RemoteMachine RemoteMachine
    {
        get => _remoteMachine;
        set { SetField(ref _remoteMachine, value); }
    }

    [JsonPropertyName("Current Theme")]
    public AppTheme CurrentTheme
    {
        get => _currentTheme;
        set { SetField(ref _currentTheme, value); }
    }

    [JsonPropertyName("Theme List")]
    public List<AppTheme> ThemesList
    {
        get => _themesList;
        set { SetField(ref _themesList, value); }
    }

    public override string ToString()
    {
        return $"SETTINGS: Start: {StartWithSystem}, API Key: {SGDBApiKey}, Prefer Local: {PreferSteamGridDBImage}";
    }

    public void SetNewCurrentTheme(string newThemeName)
    {
        foreach (var theme in ThemesList)
        {
            if (theme.ThemeName.Equals(newThemeName))
            {
            }
        }
    }

    protected bool SetField<T>(ref T field, T value,
        [System.Runtime.CompilerServices.CallerMemberName]
        string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public virtual void OnPropertyChanged(string propertyName)
    {
        Console.WriteLine($"Settings - PropertyChanged: {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        DataHandler.WriteSettingsToFile(this);
    }
}