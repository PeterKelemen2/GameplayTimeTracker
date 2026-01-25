using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.ViewModels;

public class GameViewModel : INotifyPropertyChanged
{
    private readonly Game _game;

    public int Id => _game.Id;
    public string DisplayName => _game.DisplayName;
    public string ExePath => _game.ExePath;
    public bool IsTracked { get; set; }
    public DateTime StartTime  { get; set; }
    public DateTime EndTime  { get; set; }

    private TimeSpan _playTime;

    public TimeSpan PlayTime
    {
        get => _playTime;
        set
        {
            if (_playTime != value)
            {
                _playTime = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlayTimeText));
            }
        }
    }

    public string PlayTimeText => $"{(int)PlayTime.TotalHours:D2}h {PlayTime.Minutes:D2}m {PlayTime.Seconds}s";

    public GameViewModel(Game game)
    {
        _game = game;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}