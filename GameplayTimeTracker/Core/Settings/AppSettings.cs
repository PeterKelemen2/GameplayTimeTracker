using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker.Settings;

public class AppSettings : INotifyPropertyChanged
{
    private bool _startWithSystem = false;
    private string _sgdbApiKey = "";
    private bool _preferSGDBImages = true;
    private bool _quickAdd = false;
    private string _currentTheme = "Default";
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
        set => SetField(ref _sgdbApiKey, value);
    }

    [JsonPropertyName("Prefer SteamGridDB Image")]
    public bool PreferSteamGridDBImage
    {
        get => _preferSGDBImages;
        set => SetField(ref _preferSGDBImages, value);
    }

    [JsonPropertyName("Quick Add")]
    public bool QuickAdd
    {
        get => _quickAdd;
        set => SetField(ref _quickAdd, value);
    }

    [JsonPropertyName("Current Theme")]
    public string CurrentTheme
    {
        get => _currentTheme;
        // set => SetField(ref _currentTheme, value);
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                OnPropertyChanged(nameof(CurrentTheme));
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