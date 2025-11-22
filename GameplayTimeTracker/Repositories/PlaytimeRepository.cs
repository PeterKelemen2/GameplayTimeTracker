using System;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class PlaytimeRepository : Repository<Playtime>
{
    public PlaytimeRepository(AppDbContext db) : base(db)
    {
    }

    public void AddPlaytime(Playtime playtime)
    {
        if (playtime == null)
        {
            Console.WriteLine("Playtime is null");
            return;
        }

        if (playtime.GameId == 0)
        {
            Console.WriteLine("Playtime has no GameId!");
            return;
        }

        if (playtime.Id == 0)
        {
            playtime.CreatedOn = DateTime.Now;
        }

        _db.Playtimes.Add(playtime);
        _db.SaveChanges();
    }
}