using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using GameplayTimeTracker.Extensions;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Services;
using Microsoft.Extensions.Logging;

namespace GameplayTimeTracker.ViewModels;

public class GameViewModel : INotifyPropertyChanged
{
    private readonly ILogger<GameViewModel> _logger;
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
            if (_lastPlaytimeDur == value) return;

            _lastPlaytimeDur = value;
            _logger.LogInformation($"Last PlaytimeDur: {_lastPlaytimeDur.GetPretty()}");
        }
    }

    public TimeSpan TotalPlaytimeDur
    {
        get => _totalPlaytimeDur;
        set
        {
            if (_totalPlaytimeDur == value) return;

            _totalPlaytimeDur = value;
            _logger.LogInformation($"Total PlaytimeDur: {_totalPlaytimeDur.GetPretty()}");
        }
    }


    public GameViewModel(Game game)
    {
        _game = game;
        _logger = AppLogger.CreateLogger<GameViewModel>();
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

        LastPlaytimeDur = LastPlaytimeDur.Add(TimeSpan.FromSeconds(1));
        TotalPlaytimeDur = TotalPlaytimeDur.Add(TimeSpan.FromSeconds(1));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}