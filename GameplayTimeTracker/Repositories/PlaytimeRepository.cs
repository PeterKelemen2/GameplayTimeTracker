using System;
using System.Collections.Generic;
using System.Linq;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Extensions;
using GameplayTimeTracker.Helpers;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Services;
using GameplayTimeTracker.Validators;

namespace GameplayTimeTracker.Repositories;

public class PlaytimeRepository : Repository<Playtime>
{
    public PlaytimeRepository(AppDbContext db) : base(db)
    {
    }

    public void AddPlaytime(Playtime playtime)
    {
        var validator = new PlaytimeValidator();
        var errors = validator.Validate(playtime).ToList();

        if (errors.Any())
        {
            Console.WriteLine("Playtime validation failed:");
            foreach (var err in errors)
                Console.WriteLine($" - {err}");

            return;
        }

        GlobalServices.Repositories.PlaytimeHistoryRepository.AddToPlaytimeHistoryByPlaytime(playtime);

        _db.Playtimes.Add(playtime);
        _db.SaveChanges();
    }
}