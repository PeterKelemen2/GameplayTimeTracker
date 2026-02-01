using System.Collections.ObjectModel;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Repositories;
using GameplayTimeTracker.Services;
using GameplayTimeTracker.Services.Tracker;
using Microsoft.Extensions.Logging;

namespace GameplayTimeTracker.ViewModels;

public class MainWindowViewModel
{
    private readonly SettingsRepository _settingsRepository;
    private readonly ThemeRepository _themeRepository;
    private readonly RemoteMachineRepository _remoteMachineRepository;
    private readonly GameTrackingManager _trackingManager;
    private readonly GameRepository _gameRepository;
    private readonly PlaytimeRepository _playtimeRepository;
    private readonly ILogger<MainWindowViewModel> _logger;

    public ThemeViewModel CurrentTheme { get; set; }

    public ObservableCollection<GameViewModel> Games { get; } = new();

    public MainWindowViewModel(
        GameTrackingManager trackingManager,
        GameRepository gameRepository,
        PlaytimeRepository playtimeRepository,
        RemoteMachineRepository remoteMachineRepository,
        ThemeRepository themeRepository,
        SettingsRepository settingsRepository)
    {
        _trackingManager = trackingManager;
        _gameRepository = gameRepository;
        _playtimeRepository = playtimeRepository;
        _remoteMachineRepository = remoteMachineRepository;
        _themeRepository = themeRepository;
        _settingsRepository = settingsRepository;
        _logger = AppLogger.LoggerFactory.CreateLogger<MainWindowViewModel>();

        CurrentTheme = _settingsRepository.GetThemeViewModel();
    }

    public void OnLoaded()
    {
        _logger.LogInformation("Main window loaded");

        Games.Clear();
        foreach (var game in _gameRepository.GetAll())
        {
            GameViewModel gameVm = new(game)
            {
                TotalPlaytimeDur = _playtimeRepository.GetTotalPlaytimeDurationByGameId(game.Id),
                LastPlaytime = _playtimeRepository.GetLastPlaytimeByGameId(game.Id) ?? new Playtime()
            };

            Games.Add(gameVm);
        }

        _trackingManager.StartListening(Games);
    }
}