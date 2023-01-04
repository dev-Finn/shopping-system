using MassTransit;
using Ordering.Contracts.Commands;
using Warehouse.Contracts.Events;

namespace Warehouse.UnitTests.Tests;

public sealed class ReserveStockHandlerShould : WarehouseTestHarness
{
    [Test]
    public async Task Consume_ReserveStock_Command()
    {
        await TestHarness.Bus.Publish(new ReserveStock(NewId.NextGuid(), TestData.GetValidOrderItems(5)));
        Assert.That(await TestHarness.Consumed.Any<ReserveStock>(), Is.True);
    }

    [Test]
    public async Task Publish_StockReserved_Event()
    {
        await TestHarness.Bus.Publish(new ReserveStock(NewId.NextGuid(), TestData.GetValidOrderItems(5)));
        Assert.That(await TestHarness.Published.Any<StockReserved>(), Is.True);
    }
}
