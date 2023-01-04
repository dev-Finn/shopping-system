using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Shared.NUnit;

public sealed class TestLogger : ILogger, IDisposable
{
    public IDisposable BeginScope<TState>(TState state)
    {
        return this;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        if (formatter == null) throw new ArgumentNullException(nameof(formatter));

        var message = formatter(state, exception);

        if (string.IsNullOrEmpty(message)) return;

        message = $"[{DateTime.Now:HH:mm:ss.fff}] [{logLevel}] {message}";

        TestContext.WriteLine(message);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.Debug;
    }

    public void Dispose()
    {
    }
}

public class TestLoggerFactory : ILoggerFactory
{
    public ILogger CreateLogger(string name)
    {
        return new TestLogger();
    }

    public void AddProvider(ILoggerProvider provider)
    {
    }

    public void Dispose()
    {
    }
}
