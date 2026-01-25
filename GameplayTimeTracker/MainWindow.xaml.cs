using System.Windows;
using System.Windows.Controls.Primitives;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Services;
using GameplayTimeTracker.Services.Tracker;
using Window = System.Windows.Window;

namespace GameplayTimeTracker;

public partial class MainWindow : Window
{
    GameTrackingManager _gameTrackingManager = new GameTrackingManager();

    public MainWindow()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        Game testGame = new Game
        {
            DisplayName = "Notepad++",
            ExePath =
                "C:\\Program Files\\Notepad++\\notepad++.exe"
        };

        var gameList = GlobalServices.Repositories.GameRepository.GetAll();
        _gameTrackingManager.StartListening(gameList);

        // trackerService.StartListening(testGame);

        // GlobalServices.Repositories.GameRepository.AddOrUpdate(testGame);

        // GlobalServices.Repositories.GameRepository.DeleteById(1);
        //
        // var games = GlobalServices.Repositories.GameRepository.GetAllGames();
        //
        // foreach (var game in games)
        // {
        //     Console.WriteLine(game);
        // }

        // Playtime playtime = new Playtime
        // {
        //     GameId = 1,
        //     StartDate = new DateTime(2025, 11, 23, 23, 30, 0),
        //     EndDate = new DateTime(2025, 11, 25, 01, 30, 0),
        // };
        //
        // GlobalServices.Repositories.PlaytimeRepository.AddPlaytime(playtime);

        // GlobalServices.Repositories.PlaytimeHistoryRepository.DeletePlaytimeHistoryByGameId(1);
    }
}