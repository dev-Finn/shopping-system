using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Shared.NUnit;

[TestFixture, FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public abstract class MassTransitTestHarness
{
    private ISchedulerFactory _scheduler;
    private TimeSpan _testOffset;
    protected ITestHarness TestHarness { get; }
    protected ServiceProvider Provider { get; }


    protected MassTransitTestHarness()
    {
        InterceptQuartzSystemTime();

        IServiceCollection services = new ServiceCollection()
            .AddSingleton<ILoggerFactory>(_ => new TestLoggerFactory())
            .AddMassTransitTestHarness(configurator =>
            {

                configurator.AddDelayedMessageScheduler();
                ConfigureMassTransit(configurator);
                configurator.UsingInMemory((ctx, cfg) =>
                {
                    cfg.UseInMemoryScheduler(out _scheduler);
                    cfg.ConfigureEndpoints(ctx);
                });
            });

        ConfigureServices(services);

        Provider = services
            .BuildServiceProvider(true);

        ConfigureLogging();

        TestHarness = Provider.GetRequiredService<ITestHarness>();
    }

    [SetUp]
    public async Task Setup()
    {
        await TestHarness.Start();
    }

    [TearDown]
    public async Task Teardown()
    {
        await TestHarness.Stop();
        await TestHarness.InactivityTask;
        await TestHarness.OutputTimeline(TestContext.Out, conf => conf.IncludeAddress());
        RestoreDefaultQuartzSystemTime();
    }

    protected abstract void ConfigureMassTransit(IBusRegistrationConfigurator configurator);

    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }

    protected async Task AdvanceSystemTime(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(duration));

        var scheduler = await _scheduler.GetScheduler().ConfigureAwait(false);

        await scheduler.Standby().ConfigureAwait(false);

        _testOffset += duration;

        await scheduler.Start().ConfigureAwait(false);
    }

    private void ConfigureLogging()
    {
        var loggerFactory = Provider.GetRequiredService<ILoggerFactory>();

        LogContext.ConfigureCurrentLogContext(loggerFactory);
        Quartz.Logging.LogContext.SetCurrentLogProvider(loggerFactory);
    }

    private void InterceptQuartzSystemTime()
    {
        SystemTime.UtcNow = GetUtcNow;
        SystemTime.Now = GetNow;
    }

    private static void RestoreDefaultQuartzSystemTime()
    {
        SystemTime.UtcNow = () => DateTimeOffset.UtcNow;
        SystemTime.Now = () => DateTimeOffset.Now;
    }

    private DateTimeOffset GetUtcNow()
    {
        return DateTimeOffset.UtcNow + _testOffset;
    }

    private DateTimeOffset GetNow()
    {
        return DateTimeOffset.Now + _testOffset;
    }
}
