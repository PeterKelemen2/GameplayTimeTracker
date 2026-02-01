using System.Linq;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Repositories;

public class SettingsRepository(AppDbContext db) : Repository<Settings>(db)
{
    public Theme GetTheme()
    {
        int themeId = db.Settings.FirstOrDefault()?.ThemeId ?? 0;

        return (themeId != 0 ? Db.Set<Theme>().Find(themeId) : new Theme()) ?? new Theme();
    }
}