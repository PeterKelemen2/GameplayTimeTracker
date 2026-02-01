using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace GameplayTimeTracker.Converters;

public class SimpleBracketConsoleFormatter : ConsoleFormatter
{
    public SimpleBracketConsoleFormatter()
        : base(nameof(SimpleBracketConsoleFormatter))
    {
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var timestamp = DateTime.Now.ToString("yyyy.MM.dd-HH:mm:ss");
        var category = logEntry.Category;
        var eventId = logEntry.EventId.Id;
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        if (string.IsNullOrEmpty(message))
            return;

        textWriter.WriteLine(
            $"[{category}[{eventId}]] [{timestamp}] - {message}");
    }
}