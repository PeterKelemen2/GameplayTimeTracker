using System.Linq;
using AutoMapper;
using GameplayTimeTracker.Data;
using GameplayTimeTracker.Models;
using GameplayTimeTracker.ViewModels;

namespace GameplayTimeTracker.Repositories;

public class SettingsRepository : Repository<Settings>
{
    private readonly IMapper _mapper;

    public SettingsRepository(AppDbContext db, IMapper mapper) : base(db)
    {
        _mapper = mapper;
    }
    
    public Theme GetTheme()
    {
        int themeId = Db.Settings.FirstOrDefault()?.ThemeId ?? 0;
        
        var theme = (themeId != 0 ? Db.Set<Theme>().Find(themeId) : new Theme()) ?? new Theme();
        return theme;
    }

    public ThemeViewModel GetThemeViewModel()
    {
        return new ThemeViewModel();
        
        // var theme = GetTheme();
        // return _mapper.Map<ThemeViewModel>(theme);
    }
}