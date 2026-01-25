using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class SettingsRepository(AppDbContext db) : Repository<Settings>(db)
{
}