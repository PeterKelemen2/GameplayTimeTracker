using System.Windows;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Services;
using Window = System.Windows.Window;

namespace GameplayTimeTracker;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    public void OnLoaded(object sender, RoutedEventArgs e)
    {
        Game testGame = new Game
        {
            Name = "TestGame",
        };

        GlobalServices.Repositories.GameRepository.Add(testGame);

        // GlobalServices.Repositories.GameRepository.DeleteGameById(2);
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

        GlobalServices.Repositories.PlaytimeHistoryRepository.DeletePlaytimeHistoryByGameId(1);
    }
}