using System;
using GameplayTimeTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace GameplayTimeTracker.Data;

public class AppDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Playtime> Playtimes { get; set; }
    public DbSet<PlaytimeHistory> PlaytimeHistories { get; set; }
    public DbSet<Settings> Settings { get; set; }
    public DbSet<SettingsProfile> SettingsProfile { get; set; }
    public DbSet<Theme> Themes { get; set; }
    public DbSet<RemoteMachine> RemoteMachines { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // var folder = AppContext.BaseDirectory;
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var path = System.IO.Path.Combine(folder, "GameplayTimeTracker", "Data", "GameplayTimeTracker.db");

        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path)!);

        optionsBuilder.UseSqlite($"Data Source={path}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasOne(g => g.LastPlaytime)
            .WithOne()
            .HasForeignKey<Game>(g => g.LastPlaytimeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.TotalPlaytime)
            .WithOne()
            .HasForeignKey<Game>(g => g.TotalPlaytimeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SettingsProfile>()
            .HasOne<Settings>()
            .WithMany()
            .HasForeignKey(sp => sp.SettingsId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Settings>()
            .HasOne<Theme>()
            .WithMany()
            .HasForeignKey(sp => sp.ThemeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Settings>()
            .HasOne<RemoteMachine>()
            .WithMany()
            .HasForeignKey(sp => sp.RemoteMachineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}