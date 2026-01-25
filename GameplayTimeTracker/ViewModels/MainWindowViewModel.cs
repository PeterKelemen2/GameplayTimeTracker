using GameplayTimeTracker.Repositories;
using GameplayTimeTracker.Services.Tracker;
using Microsoft.Extensions.Logging;

namespace GameplayTimeTracker.ViewModels;

public class MainWindowViewModel
{
    private readonly GameTrackingManager _trackingManager;
    private readonly GameRepository _gameRepository;
    private readonly ILogger<MainWindowViewModel> _logger;

    public MainWindowViewModel(
        GameTrackingManager trackingManager,
        GameRepository gameRepository,
        ILogger<MainWindowViewModel> logger)
    {
        _trackingManager = trackingManager;
        _gameRepository = gameRepository;
        _logger = logger;
    }

    public void OnLoaded()
    {
        _logger.LogInformation("Main window loaded");

        var games = _gameRepository.GetAll();

        _trackingManager.StartListening(games);
    }
}