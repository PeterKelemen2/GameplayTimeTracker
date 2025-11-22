using System.Linq;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;
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
    
    
}