using System.Collections.Generic;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.ViewModels;

namespace GameplayTimeTracker.Services.Tracker;

public class GameTrackingManager
{
    private readonly List<SingleProcessTracker> _trackers = new();

    public void StartListening(IEnumerable<GameViewModel> games)
    {
        foreach (var game in games)
        {
            var tracker = new SingleProcessTracker(game);
            tracker.Start();
            _trackers.Add(tracker);
        }
    }

    public void StopListening()
    {
        foreach (var tracker in _trackers)
            tracker.Stop();

        _trackers.Clear();
    }
}