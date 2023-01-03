using MassTransit;
using Ordering.Contracts.Commands;
using Warehouse.Contracts.Events;
using Warehouse.UnitTests.Contracts;

namespace Warehouse.UnitTests.Tests;

public sealed class ShipOrderHandlerShould : WarehouseTestHarness
{
    [Test]
    public async Task Consume_ShipOrder_Command()
    {
        await TestHarness.Bus.Publish(new ShipOrder(NewId.NextGuid(), TestData.GetValidOrderItems(5)));
        Assert.That(await TestHarness.Consumed.Any<ShipOrder>(), Is.True);
    }

    [Test]
    public async Task Publishes_OrderShipped_Event()
    {
        await TestHarness.Bus.Publish(new ShipOrder(NewId.NextGuid(), TestData.GetValidOrderItems(5)));
        Assert.That(await TestHarness.Published.Any<OrderShipped>(), Is.True);
    }
}
