using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class SettingsRepository : Repository<Settings>
{
    public SettingsRepository(AppDbContext db) : base(db)
    {
    }
}