using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker.Settings;

public class RemoteMachine : INotifyPropertyChanged
{
    private string _address;
    private int _port = 22;
    private string _user;
    private string _password;
    private string _remoteFolder;
    private int _retainForDays = 30;

    [JsonPropertyName("Address")]
    public string Address
    {
        get => _address;
        set { SetField(ref _address, value); }
    }

    [JsonPropertyName("Port")]
    public int Port
    {
        get => _port;
        set { SetField(ref _port, value); }
    }

    [JsonPropertyName("User")]
    public string User
    {
        get => _user;
        set { SetField(ref _user, value); }
    }

    [JsonPropertyName("Password")]
    public string Password
    {
        get => _password;
        set { SetField(ref _password, value); }
    }

    [JsonPropertyName("Remote Folder")]
    public string RemoteFolder
    {
        get => _remoteFolder;
        set { SetField(ref _remoteFolder, value); }
    }

    [JsonPropertyName("Retain For Days")]
    public int RetainForDays
    {
        get => _retainForDays;
        set { SetField(ref _retainForDays, value); }
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
        Console.WriteLine($"Remote Machine - PropertyChanged: {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        DataHandler.WriteSettingsToFile(Common.Settings);
    }
}