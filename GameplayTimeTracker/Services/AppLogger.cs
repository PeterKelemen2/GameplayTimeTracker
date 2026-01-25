using Microsoft.Extensions.Logging;

namespace GameplayTimeTracker.Services;

public static class AppLogger
{
    public static ILoggerFactory LoggerFactory { get; } = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
    {
        builder
            .AddConsole()
            .AddDebug()
            .SetMinimumLevel(LogLevel.Information); // default minimum level
    });

    public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
}