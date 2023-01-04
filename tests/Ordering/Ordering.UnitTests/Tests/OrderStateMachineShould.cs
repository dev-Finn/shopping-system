using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;

namespace Ordering.UnitTests.Tests;

public sealed class OrderStateMachineShould : OrderStateMachineTestHarness
{
    [Test]
    public async Task Create_State_Instance_when_OrderSubmitted_is_received()
    {
        var validEvent = TestData.GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(validEvent);
        Assert.That(SagaHarness.Created.Select(x => x.CorrelationId == validEvent.OrderId).Any, Is.True);
    }

    [Test]
    public async Task Publish_ReserveStock_when_OrderSubmitted_is_received()
    {
        var submitOrder = TestData.GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(submitOrder);

        Assert.That(await TestHarness.Published.Any<ReserveStock>(), Is.True);
    }

    [Test]
    public async Task Publish_ProcessPayment_when_StockReserved_is_received()
    {
        var submitOrder = TestData.GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(submitOrder);
        Assert.That(await TestHarness.Published.Any<ReserveStock>(), Is.True);

        var stockReserved = TestData.GetValidStockReservedEvent(submitOrder.OrderId);
        await TestHarness.Bus.Publish(stockReserved);

        Assert.That(await TestHarness.Published.Any<ProcessPayment>(), Is.True);
    }

    [Test]
    public async Task Publish_ShipOrder_when_PaymentProcessed_is_received()
    {
        var submitOrder = TestData.GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(submitOrder);
        Assert.That(await TestHarness.Published.Any<ReserveStock>(), Is.True);

        var stockReserved = TestData.GetValidStockReservedEvent(submitOrder.OrderId);
        await TestHarness.Bus.Publish(stockReserved);
        Assert.That(await TestHarness.Published.Any<ProcessPayment>(), Is.True);

        var paymentProcessed = TestData.GetValidPaymentProcessedEvent(submitOrder.OrderId);
        await TestHarness.Bus.Publish(paymentProcessed);

        Assert.That(await TestHarness.Published.Any<ShipOrder>(), Is.True);
    }

    [Test]
    public async Task Publish_OrderCancelled_when_StockReservationTimeout_is_received()
    {
        var submitOrder = TestData.GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(submitOrder);
        Assert.That(await TestHarness.Published.Any<ReserveStock>(), Is.True);

        var stockReserved = TestData.GetValidStockReservedEvent(submitOrder.OrderId);
        await TestHarness.Bus.Publish(stockReserved);
        Assert.That(await TestHarness.Published.Any<ProcessPayment>(), Is.True);

        await AdvanceSystemTime(TimeSpan.FromDays(7));

        Assert.That(await TestHarness.Published.Any<OrderCancelled>(), Is.True);
    }

    [Test]
    public async Task Publish_OrderCancelled_when_CancelOrder_is_received()
    {
        var submitOrder = TestData.GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(submitOrder);
        Assert.That(await TestHarness.Published.Any<ReserveStock>(), Is.True);

        var cancelOrder = TestData.GetValidCancelOrderCommand(submitOrder.OrderId);
        await TestHarness.Bus.Publish(cancelOrder);

        Assert.That(await TestHarness.Published.Any<OrderCancelled>(), Is.True);
    }

    [Test]
    public async Task Publish_OrderNotFound_when_invalid_CancelOrder_is_received()
    {
        var submitOrder = TestData.GetValidOrderSubmittedEvent();
        await TestHarness.Bus.Publish(submitOrder);
        Assert.That(await TestHarness.Published.Any<ReserveStock>(), Is.True);

        var cancelOrder = TestData.GetCancelOrderCommand();
        await TestHarness.Bus.Publish(cancelOrder);

        Assert.That(await TestHarness.Published.Any<OrderCancelled>(), Is.False);
    }
}
