using GameplayTimeTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace GameplayTimeTracker.Data;

public class AppDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Playtime> Playtimes { get; set; }
    public DbSet<PlaytimeHistory> PlaytimeHistories { get; set; }
    public DbSet<Settings> Settings { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Data/gameplaytimetracker.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasOne(g => g.LastPlaytime)
            .WithOne()
            .HasForeignKey<Game>(g => g.LastPlaytimeId);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.TotalPlaytime)
            .WithOne()
            .HasForeignKey<Game>(g => g.TotalPlaytimeId);
    }
}