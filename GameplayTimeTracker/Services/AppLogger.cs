using GameplayTimeTracker.Converters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace GameplayTimeTracker.Services;

public static class AppLogger
{
    public static ILoggerFactory LoggerFactory { get; }
        = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder
                .ClearProviders()
                .AddConsole(options =>
                {
                    options.FormatterName = nameof(SimpleBracketConsoleFormatter);
                })
                .AddConsoleFormatter<SimpleBracketConsoleFormatter, ConsoleFormatterOptions>()
                .AddDebug()
                .SetMinimumLevel(LogLevel.Information);
        });

    public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
}