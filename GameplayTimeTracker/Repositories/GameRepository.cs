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

    public bool ExistsById(int id) => Db.Games.Any(g => g.Id == id);

    public Game? GetGameById(int id) => Db.Games
        .Include(g => g.LastPlaytime)
        .Include(g => g.TotalPlaytime)
        .FirstOrDefault(g => g.Id == id);

    public List<Game> GetAllGames() => Db.Games.ToList();

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

            Db.Games.Add(game);
            Db.SaveChanges();
        }
        else
        {
            Db.SaveChanges();
        }
    }

    public void DeleteGameById(int id)
    {
        var game = GetGameById(id);
        if (game != null) DeleteGame(game);
    }

    public void DeleteGame(Game? game)
    {
        if (game != null)
        {
            Db.Games.Remove(game);
            Db.SaveChanges();
        }
        else
        {
            Console.WriteLine("Game not found");
        }
    }
}