using System;
using System.Linq;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Services;
using GameplayTimeTracker.Validators;

namespace GameplayTimeTracker.Repositories;

public class PlaytimeRepository(AppDbContext db) : Repository<Playtime>(db)
{
    private readonly PlaytimeValidator _validator = new PlaytimeValidator();

    public void AddPlaytime(Playtime playtime)
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

        GlobalServices.Repositories.PlaytimeHistoryRepository.AddToPlaytimeHistoryByPlaytime(playtime);

        Db.Playtimes.Add(playtime);
        Db.SaveChanges();
    }
}