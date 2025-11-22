using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class PlaytimeHistoryRepository : Repository<PlaytimeHistory>
{
    public PlaytimeHistoryRepository(AppDbContext db) : base(db)
    {
    }
}