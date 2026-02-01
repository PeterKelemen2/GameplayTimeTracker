using System.Collections.ObjectModel;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Repositories;
using GameplayTimeTracker.Services;
using GameplayTimeTracker.Services.Tracker;
using Microsoft.Extensions.Logging;

namespace GameplayTimeTracker.ViewModels;

public class MainWindowViewModel
{
    private readonly GameTrackingManager _trackingManager;
    private readonly GameRepository _gameRepository;
    private readonly PlaytimeRepository _playtimeRepository;
    private readonly ILogger<MainWindowViewModel> _logger;

    public ObservableCollection<GameViewModel> Games { get; } = new();

    public MainWindowViewModel(
        GameTrackingManager trackingManager,
        GameRepository gameRepository,
        PlaytimeRepository playtimeRepository)
    {
        _trackingManager = trackingManager;
        _gameRepository = gameRepository;
        _playtimeRepository = playtimeRepository;
        _logger = AppLogger.LoggerFactory.CreateLogger<MainWindowViewModel>();
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