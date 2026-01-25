using System.Collections.ObjectModel;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Repositories;
using GameplayTimeTracker.Services.Tracker;
using Microsoft.Extensions.Logging;

namespace GameplayTimeTracker.ViewModels;

public class MainWindowViewModel
{
    private readonly GameTrackingManager _trackingManager;
    private readonly GameRepository _gameRepository;
    private readonly ILogger<MainWindowViewModel> _logger;
    
    public ObservableCollection<GameViewModel> Games { get; } = new();
    
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

        Games.Clear();
        foreach (var game in _gameRepository.GetAll())
        {
            Games.Add(new GameViewModel(game));
        }

        _trackingManager.StartListening(Games);
    }
}