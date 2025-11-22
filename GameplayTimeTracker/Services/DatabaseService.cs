using GameplayTimeTracker.Data;

namespace GameplayTimeTracker.Services;

public class DatabaseService
{
    private readonly AppDbContext _db;

    public DatabaseService()
    {
        _db = new AppDbContext();
        _db.Database.EnsureCreated();
    }
}