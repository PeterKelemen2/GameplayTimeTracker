using System;
using System.Collections.Generic;
using System.Linq;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Extensions;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Services;
using GameplayTimeTracker.Validators;

namespace GameplayTimeTracker.Repositories;

public class PlaytimeRepository(AppDbContext db) : Repository<Playtime>(db)
{
    private readonly PlaytimeValidator _validator = new PlaytimeValidator();

    public override void Add(Playtime playtime)
    {
        // var validator = new PlaytimeValidator();
        var errors = _validator.Validate(playtime).ToList();

        if (errors.Any())
        {
            Console.WriteLine("Playtime validation failed:");
            foreach (var err in errors)
                Console.WriteLine($" - {err}");

            return;
        }

        Db.Playtimes.Add(playtime);
        Db.SaveChanges();
    }

    private TimeSpan SumDurations(IEnumerable<Playtime> playtimes)
    {
        return playtimes
            .AsEnumerable()
            .Select(p => p.GetDuration())
            .Aggregate(TimeSpan.Zero, (acc, dur) => acc.Add(dur));
    }

    public TimeSpan GetTotalPlaytimeDurationByGameId(int gameId)
    {
        return SumDurations(Db.Playtimes.Where(x => x.GameId == gameId));
    }

    public TimeSpan GetTotalPlaytimeDuration()
    {
        return SumDurations(Db.Playtimes);
    }

    public Playtime? GetLastPlaytimeByGameId(int gameId)
    {
        return Db.Playtimes
            .Where(x => x.GameId == gameId)
            .OrderByDescending(x => x.EndDate) // latest end date first
            .FirstOrDefault();
    }

    public TimeSpan? GetLastPlaytimeDurationByGameId(int gameId)
    {
        var lastPlaytime = Db.Playtimes
            .Where(x => x.GameId == gameId)
            .OrderByDescending(x => x.EndDate)
            .FirstOrDefault();

        return lastPlaytime?.GetDuration();
    }
}