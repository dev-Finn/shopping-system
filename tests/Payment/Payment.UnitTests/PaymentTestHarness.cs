using MassTransit;
using Payment.Components;
using Shared.NUnit;

namespace Payment.UnitTests;

public abstract class PaymentTestHarness : MassTransitTestHarness
{
    protected override void ConfigureTestHarness(IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumers(typeof(ComponentsAssemblyMarker).Assembly);
        configurator.AddActivities(typeof(ComponentsAssemblyMarker).Assembly);
        configurator.AddSagaStateMachines(typeof(ComponentsAssemblyMarker).Assembly);
    }
}
