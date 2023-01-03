using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shared.NUnit;

public abstract class MassTransitTestHarness
{
    protected ITestHarness TestHarness { get; }
    public ServiceProvider Provider { get; }


    public MassTransitTestHarness()
    {
        IServiceCollection services = new ServiceCollection()
            .AddSingleton(typeof(ILogger<>), typeof(TestLogger<>))
            .AddMassTransitTestHarness(ConfigureTestHarness);

        Provider = services
            .BuildServiceProvider(true);

        TestHarness =  Provider.GetRequiredService<ITestHarness>();
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
    }

    protected abstract void ConfigureTestHarness(IBusRegistrationConfigurator configurator);
}
