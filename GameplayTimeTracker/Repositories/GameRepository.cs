using System;
using System.Collections.Generic;
using System.Linq;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.Services;
using Microsoft.EntityFrameworkCore;

namespace GameplayTimeTracker.Repositories;

public class GameRepository : Repository<Game>
{
    public GameRepository(AppDbContext db) : base(db)
    {
    }

    public Game? GetGameById(int id) => _db.Games
        .Include(g => g.LastPlaytime)
        .Include(g => g.TotalPlaytime)
        .FirstOrDefault(g => g.Id == id);

    public List<Game> GetAllGames() => _db.Games.ToList();

    public void AddGame(Game game)
    {
        Console.WriteLine($"Adding game: {game}");

        if (game.Id == 0)
        {
            game.CreatedOn = DateTime.Now;

            Playtime lastPlaytime = new Playtime { CreatedOn = DateTime.Now };
            Playtime totalPlaytime = new Playtime { CreatedOn = DateTime.Now };

            game.LastPlaytime = lastPlaytime;
            game.TotalPlaytime = totalPlaytime;

            _db.Games.Add(game);
            _db.SaveChanges();
        }
        else
        {
            _db.SaveChanges();
        }
    }

    public void DeleteGameById(int id)
    {
        var game = GetGameById(id);
        DeleteGame(game);
    }

    public void DeleteGame(Game game)
    {
        if (game != null)
        {
            _db.Games.Remove(game);
            _db.SaveChanges();
        }
        else
        {
            Console.WriteLine("Game not found");
        }
    }
}