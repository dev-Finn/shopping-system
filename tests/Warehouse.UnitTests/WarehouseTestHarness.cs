using MassTransit;
using Shared.NUnit;
using Warehouse.Components;

namespace Warehouse.UnitTests;

public abstract class WarehouseTestHarness : MassTransitTestHarness
{
    protected override void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumers(typeof(ComponentsAssemblyMarker).Assembly);
        configurator.AddActivities(typeof(ComponentsAssemblyMarker).Assembly);
        configurator.AddSagaStateMachines(typeof(ComponentsAssemblyMarker).Assembly);
    }
}
