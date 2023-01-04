using MassTransit;
using Ordering.Components;
using Shared.NUnit;

namespace Ordering.UnitTests;

public abstract class OrderingTestHarness : MassTransitTestHarness
{
    protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumers(typeof(ComponentsAssemblyMarker).Assembly);
        configurator.AddActivities(typeof(ComponentsAssemblyMarker).Assembly);
        configurator.AddSagaStateMachines(typeof(ComponentsAssemblyMarker).Assembly);
    }
}
