using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.ViewModels;

public class GameViewModel : INotifyPropertyChanged
{
    private readonly Game _game;
    private DispatcherTimer _timer;
    public int Id => _game.Id;
    public string DisplayName => _game.DisplayName;
    public string ExePath => _game.ExePath;

    private bool _isTracked;

    public bool IsTracked
    {
        get => _isTracked;
        set
        {
            if (_isTracked != value)
            {
                _isTracked = value;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(IsTracked));


            if (_isTracked)
                StartTimer();
            else
                StopTimer();
        }
    }

    public Playtime LastPlaytime { get; set; }


    private TimeSpan _lastPlaytimeDur;
    private TimeSpan _totalPlaytimeDur;

    public TimeSpan LastPlaytimeDur
    {
        get => _lastPlaytimeDur;
        set
        {
            if (_lastPlaytimeDur != value)
            {
                _lastPlaytimeDur = value;
            }
        }
    }

    public TimeSpan TotalPlaytimeDur
    {
        get => _totalPlaytimeDur;
        set
        {
            if (_totalPlaytimeDur != value)
            {
                _totalPlaytimeDur = value;
            }
        }
    }


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

    public string PlayTimeText => $"{(int)PlayTime.TotalHours:D2}h {PlayTime.Minutes:D2}m {PlayTime.Seconds:D2}s";

    public GameViewModel(Game game)
    {
        _game = game;
    }

    private void StartTimer()
    {
        if (_timer != null) return;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void StopTimer()
    {
        if (_timer == null) return;

        _timer.Stop();
        _timer.Tick -= Timer_Tick;
        _timer = null;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (!_isTracked)
        {
            StopTimer();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}