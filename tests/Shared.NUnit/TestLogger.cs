using Microsoft.Extensions.Logging;

namespace Shared.NUnit;

public sealed class TestLogger<T> : ILogger<T>, ILoggerProvider
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        TestContext.WriteLine($"[{logLevel}] {formatter(state, exception)}");
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return this;
    }

    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return this;
    }
}
