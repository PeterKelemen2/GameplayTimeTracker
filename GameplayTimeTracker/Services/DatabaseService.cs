using System;
using System.Linq;
using System.Windows;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;
using Microsoft.EntityFrameworkCore;
using Settings = GameplayTimeTracker.Models.Settings;

namespace GameplayTimeTracker.Services;

public class DatabaseService
{
    private readonly AppDbContext _db;
    private const string APP_VERSION = "1.0.0";

    public DatabaseService(AppDbContext db)
    {
        _db = db;
        EnsureDatabaseAndSchema();
        SeedBaseData();
    }

    private void EnsureDatabaseAndSchema()
    {
        try
        {
            var pendingMigrations = _db.Database.GetPendingMigrations().ToList();
            
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Applying {pendingMigrations.Count} pending migrations:");
                foreach (var migration in pendingMigrations)
                {
                    Console.WriteLine($"  - {migration}");
                }
                
                BackupDatabase();
                _db.Database.Migrate();
                
                Console.WriteLine("✓ Migrations applied successfully");
            }
            else
            {
                // Ensure database exists even if no pending migrations
                _db.Database.Migrate();
                Console.WriteLine("✓ Database schema is up to date");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Migration failed: {ex.Message}");
            RestoreBackup();
            
            MessageBox.Show(
                $"Database migration failed. The application will now close.\n\nError: {ex.Message}",
                "Database Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            System.Windows.Application.Current.Shutdown();
        }
    }

    private void BackupDatabase()
    {
        try
        {
            var dbPath = GetDatabasePath();
            if (System.IO.File.Exists(dbPath))
            {
                var backupPath = dbPath.Replace(".db", $"_backup_{DateTime.Now:yyyyMMddHHmmss}.db");
                System.IO.File.Copy(dbPath, backupPath, true);
                Console.WriteLine($"✓ Backup created: {System.IO.Path.GetFileName(backupPath)}");
                
                CleanOldBackups();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Backup warning: {ex.Message}");
        }
    }

    private void CleanOldBackups()
    {
        try
        {
            var dbPath = GetDatabasePath();
            var directory = System.IO.Path.GetDirectoryName(dbPath);
            var backupFiles = System.IO.Directory.GetFiles(directory, "*_backup_*.db")
                .OrderByDescending(f => f)
                .Skip(5);
            
            foreach (var file in backupFiles)
            {
                System.IO.File.Delete(file);
            }
        }
        catch { /* Ignore cleanup errors */ }
    }

    private void RestoreBackup()
    {
        try
        {
            var dbPath = GetDatabasePath();
            var directory = System.IO.Path.GetDirectoryName(dbPath);
            var latestBackup = System.IO.Directory.GetFiles(directory, "*_backup_*.db")
                .OrderByDescending(f => f)
                .FirstOrDefault();
            
            if (latestBackup != null)
            {
                System.IO.File.Copy(latestBackup, dbPath, true);
                Console.WriteLine($"✓ Database restored from backup");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Restore failed: {ex.Message}");
        }
    }

    private string GetDatabasePath()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return System.IO.Path.Combine(folder, "GameplayTimeTracker", "Data", "GameplayTimeTracker.db");
    }

    private void SeedBaseData()
    {
        try
        {
            if (!_db.Settings.Any())
            {
                var now = DateTime.Now;

                Settings s = new Settings { CreatedOn = now };
                _db.Settings.Add(s);
                _db.SaveChanges();

                if (!_db.SettingsProfile.Any())
                {
                    _db.SettingsProfile.Add(new SettingsProfile
                    {
                        ProfileName = "Default",
                        SettingsId = s.Id,
                        CreatedOn = now
                    });
                    _db.SaveChanges();
                }
                
                Console.WriteLine("✓ Base data seeded");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Seeding error: {ex.Message}");
            throw;
        }
    }
}