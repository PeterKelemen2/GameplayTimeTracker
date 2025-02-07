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

    [JsonPropertyName("Address")]
    public string Address
    {
        get => _address;
        set
        {
            if (value != _address)
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
    }

    [JsonPropertyName("Port")]
    public int Port
    {
        get => _port;
        set
        {
            if (value != _port)
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }
    }

    [JsonPropertyName("User")]
    public string User
    {
        get => _user;
        set
        {
            if (value != _user)
            {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }
    }

    [JsonPropertyName("Password")]
    public string Password
    {
        get => _password;
        set
        {
            if (value != _password)
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
    }

    [JsonPropertyName("Remote Folder")]
    public string RemoteFolder
    {
        get => _remoteFolder;
        set
        {
            if (value != _remoteFolder)
            {
                _remoteFolder = value;
                OnPropertyChanged(nameof(RemoteFolder));
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
        Console.WriteLine($"Remote Machine - PropertyChanged: {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        DataHandler.WriteSettingsToFile(Common.Settings);
    }
}