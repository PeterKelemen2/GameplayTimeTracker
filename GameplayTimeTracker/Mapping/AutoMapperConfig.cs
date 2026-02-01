using GameplayTimeTracker.Models;
using GameplayTimeTracker.ViewModels;
using AutoMapper;
using GameplayTimeTracker.Services;

namespace GameplayTimeTracker.Mapping;

public static class AutoMapperConfig
{
    public static MapperConfiguration Configuration { get; private set; }

    static AutoMapperConfig()
    {
        Configuration = new MapperConfiguration(
            cfg =>
            {
                cfg.CreateMap<Theme, ThemeViewModel>();
            },
            AppLogger.LoggerFactory
        );
    }

    public static IMapper GetMapper() => Configuration.CreateMapper();
}