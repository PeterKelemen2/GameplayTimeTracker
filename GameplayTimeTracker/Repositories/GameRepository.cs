using System;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class GameRepository(AppDbContext db) : Repository<Game>(db)
{

}