using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker.Settings;

public class AppTheme : INotifyPropertyChanged
{
    private string _themeName = "Default";

    private ObservableDictionary<string, string> _colors = AppColors.GetColorsDict();

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
    [JsonConverter(typeof(ObservableDictionaryConverter<string, string>))]
    public ObservableDictionary<string, string> Colors
    {
        get => _colors;
        set
        {
            _colors = value;
            OnPropertyChanged(nameof(Colors));
        }
    }

    public void UpdateColor(string key, string newValue)
    {
        if (_colors.ContainsKey(key))
        {
            _colors[key] = newValue;
            OnPropertyChanged(nameof(Colors));
            OnPropertyChanged($"Color[{key}]"); // Notify changes for specific color key
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
        Console.WriteLine($"Theme ({_themeName}) - PropertyChanged: {propertyName}");

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}