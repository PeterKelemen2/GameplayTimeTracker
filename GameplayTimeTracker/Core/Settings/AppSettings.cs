using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Xceed.Wpf.AvalonDock.Themes;

namespace GameplayTimeTracker.Settings;

public class AppSettings : INotifyPropertyChanged
{
    private bool _startWithSystem = true;
    private string _sgdbApiKey = "";
    private bool _dontShowApiKeyPrompt = false;
    private bool _preferSGDBImages = true;
    private bool _quickAdd = false;

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
            if (_startWithSystem != value)
            {
                _startWithSystem = value;
                OnPropertyChanged(nameof(StartWithSystem));
                DataHandler.ManageStartupShortcut(_startWithSystem);
            }
        }
    }

    [JsonPropertyName("SteamGridDB API key")]
    public string SGDBApiKey
    {
        get => _sgdbApiKey;
        set
        {
            SetField(ref _sgdbApiKey, value);
            DataHandler.WriteSettingsToFile(this);
        }
    }

    [JsonPropertyName("Dont Show API Key prompt")]
    public bool DontShowApiKeyPrompt
    {
        get => _dontShowApiKeyPrompt;
        set
        {
            SetField(ref _dontShowApiKeyPrompt, value);
            DataHandler.WriteSettingsToFile(this);
        }
    }

    [JsonPropertyName("Prefer SteamGridDB Image")]
    public bool PreferSteamGridDBImage
    {
        get => _preferSGDBImages;
        set
        {
            SetField(ref _preferSGDBImages, value);
            DataHandler.WriteSettingsToFile(this);
        }
    }

    [JsonPropertyName("Quick Add")]
    public bool QuickAdd
    {
        get => _quickAdd;
        set
        {
            SetField(ref _quickAdd, value);
            DataHandler.WriteSettingsToFile(this);
        }
    }

    [JsonPropertyName("Display Type")]
    public GameDisplay Display
    {
        get => _gameDisplay;
        set
        {
            if (_gameDisplay != value)
            {
                _gameDisplay = value;
                OnPropertyChanged(nameof(Display));
                DataHandler.WriteSettingsToFile(this);
            }
        }
    }

    [JsonPropertyName("Saving Frequency")]
    public int SavingFrequencyInMinutes
    {
        get => _savingFreqInMin;
        set
        {
            if (_savingFreqInMin != value)
            {
                _savingFreqInMin = value;
                OnPropertyChanged(nameof(SavingFrequencyInMinutes));
                DataHandler.WriteSettingsToFile(this);
            }
        }
    }

    [JsonPropertyName("Current Theme")]
    public AppTheme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                OnPropertyChanged(nameof(CurrentTheme));
                DataHandler.WriteSettingsToFile(this);
            }
        }
    }

    [JsonPropertyName("Theme List")]
    public List<AppTheme> ThemesList
    {
        get => _themesList;
        set
        {
            if (_themesList != value)
            {
                _themesList = value;
                OnPropertyChanged(nameof(ThemesList));
                DataHandler.WriteSettingsToFile(this);
            }
        }
    }

    public override string ToString()
    {
        return $"SETTINGS: Start: {StartWithSystem}, API Key: {SGDBApiKey}, Prefer Local: {PreferSteamGridDBImage}";
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
    }
}