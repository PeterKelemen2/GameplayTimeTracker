using System;
using System.Collections.Generic;
using System.Linq;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace GameplayTimeTracker.Repositories;

public class GameRepository(AppDbContext db) : Repository<Game>(db)
{
    public override Game? GetById(int id) => Db.Games
        .Include(g => g.LastPlaytime)
        .Include(g => g.TotalPlaytime)
        .FirstOrDefault(g => g.Id == id);

    public override void Add(Game game)
    {
        Console.WriteLine($"Adding game: {game}");

        if (game.Id == 0)
        {
            game.CreatedOn = DateTime.Now;

            Playtime lastPlaytime = new Playtime { CreatedOn = DateTime.Now };
            Playtime totalPlaytime = new Playtime { CreatedOn = DateTime.Now };

            game.LastPlaytime = lastPlaytime;
            game.TotalPlaytime = totalPlaytime;

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