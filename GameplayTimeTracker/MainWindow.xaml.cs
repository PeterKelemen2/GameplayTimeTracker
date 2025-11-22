using System;
using System.Windows;
using Cairo;
using GameplayTimeTracker.Helpers;
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

        // GlobalServices.Repositories.GameRepository.AddGame(testGame);

        // GlobalServices.Repositories.GameRepository.DeleteGameById(2);
        //
        // var games = GlobalServices.Repositories.GameRepository.GetAllGames();
        //
        // foreach (var game in games)
        // {
        //     Console.WriteLine(game);
        // }

        Playtime playtime = new Playtime
        {
            GameId = 1,
            StartDate = new DateTime(2025, 11, 22, 16, 30, 0),
            EndDate = new DateTime(2025, 11, 23, 16, 30, 0),
        };
        // var dur = PlaytimeHelper.GetDurationFromDates(playtime.StartDate, playtime.EndDate);
        // Console.WriteLine("Duration: " + dur);
        playtime.Hours = 24;
        playtime.Minutes = 0;
        playtime.Seconds = 0;

        GlobalServices.Repositories.PlaytimeRepository.AddPlaytime(playtime);
    }
}