using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker.Settings;

public class AppTheme : INotifyPropertyChanged
{
    private string _themeName = "Default";
    private Dictionary<string, string> _colors = AppColors.GetColorsDict();

    [JsonPropertyName("Theme Name")]
    public string ThemeName
    {
        get => _themeName;
        set
        {
            if (_themeName != value)
            {
                _themeName = value;
                OnPropertyChanged(nameof(ThemeName));
            }
        }
    }

    [JsonPropertyName("Colors")]
    public Dictionary<string, string> Colors
    {
        get => _colors;
        set
        {
            if (SetField(ref _colors, value))
            {
                _colors = value;
                OnPropertyChanged(nameof(Colors));
            }
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

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