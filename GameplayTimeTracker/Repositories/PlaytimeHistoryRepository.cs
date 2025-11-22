using System;
using System.Linq;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class PlaytimeHistoryRepository : Repository<PlaytimeHistory>
{
    public PlaytimeHistoryRepository(AppDbContext db) : base(db)
    {
    }

    public PlaytimeHistory GetHistoryItemWithDate(DateTime date)
    {
        return _db.PlaytimeHistories.FirstOrDefault(x => x.Date.Date.Equals(date.Date));
    }
}