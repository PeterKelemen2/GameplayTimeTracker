using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class RemoteMachineRepository(AppDbContext db) : Repository<Game>(db)
{

}