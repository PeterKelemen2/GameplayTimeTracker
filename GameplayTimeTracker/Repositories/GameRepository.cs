using System;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class GameRepository(AppDbContext db) : Repository<Game>(db)
{
    public override void Add(Game game)
    {
        Console.WriteLine($"Adding game: {game}");

        if (game.Id == 0)
        {
            game.CreatedOn = DateTime.Now;

            Db.Games.Add(game);

            Db.SaveChanges();
        }
        else
        {
            Update(game);
            Db.SaveChanges();
        }
    }
}