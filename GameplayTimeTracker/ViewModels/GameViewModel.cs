using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
    private readonly object _lockObject = new object();

    public int Id => _game.Id;
    public string DisplayName => _game.DisplayName;
    public string ExePath => _game.ExePath;

    public GameViewModel(Game game)
    {
        _game = game;
        _logger = AppLogger.CreateLogger<GameViewModel>();
    }

#if DEBUG
    public GameViewModel()
    {
        _game = new Game();
        _game.DisplayName = "Sample Game";
        _game.ExePath = @"C:\Games\SampleGame.exe";
        LastPlaytimeDur = TimeSpan.FromMinutes(5);
        TotalPlaytimeDur = TimeSpan.FromHours(12);
        LastPlaytime = new Playtime
        {
            StartDate = DateTime.Now.AddMinutes(-5),
            EndDate = DateTime.Now
        };
    }
#endif

    public string IsRunningLabel => "Running!";
    private bool _isTracked;

    public bool IsTracked
    {
        get => _isTracked;
        set
        {
            if (_isTracked == value) return;

            _isTracked = value;

            OnPropertyChanged();

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged();

                if (_isTracked)
                    StartTimer();
                else
                    StopTimer();
            });
        }
    }

    private Playtime _lastPlaytime;

    public Playtime LastPlaytime
    {
        get => _lastPlaytime;
        set
        {
            lock (_lockObject)
            {
                _lastPlaytime = value;
            }
        }
    }


    public string LastPlaytimeLabel => "Last Playtime";
    private TimeSpan _lastPlaytimeDur;

    public TimeSpan LastPlaytimeDur
    {
        get => _lastPlaytimeDur;
        set
        {
            lock (_lockObject)
            {
                if (_lastPlaytimeDur == value) return;
                _lastPlaytimeDur = value;

                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    _logger.LogInformation($"[{DisplayName}] Last PlaytimeDur: {_lastPlaytimeDur.GetPretty()}");
                    OnPropertyChanged();
                });
            }
        }
    }

    public string TotalPlaytimeLabel => "Total Playtime";
    private TimeSpan _totalPlaytimeDur;

    public TimeSpan TotalPlaytimeDur
    {
        get => _totalPlaytimeDur;
        set
        {
            lock (_lockObject)
            {
                if (_totalPlaytimeDur == value) return;
                _totalPlaytimeDur = value;

                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    _logger.LogInformation($"[{DisplayName}] Total PlaytimeDur: {_totalPlaytimeDur.GetPretty()}");
                    OnPropertyChanged();
                });
            }
        }
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
        if (!IsTracked)
        {
            StopTimer();
        }

        LastPlaytimeDur += TimeSpan.FromSeconds(1);
        TotalPlaytimeDur += TimeSpan.FromSeconds(1);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}