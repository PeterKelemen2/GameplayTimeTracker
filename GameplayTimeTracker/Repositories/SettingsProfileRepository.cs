using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class SettingsProfileRepository : Repository<SettingsProfile>
{
    public SettingsProfileRepository(AppDbContext db) : base(db)
    {
    }
}