using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class ThemeRepository(AppDbContext db) : Repository<Game>(db)
{

}