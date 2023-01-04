using MassTransit;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;
using Ordering.Contracts.Models;
using Ordering.UnitTests.Contracts;
using Payment.Contracts.Events;
using Warehouse.Contracts.Events;

namespace Ordering.UnitTests;

public static class TestData
{
    internal static OrderSubmitted GetValidOrderSubmittedEvent()
    {
        return new OrderSubmitted(NewId.NextGuid(), GetValidOrderItems(Random.Shared.Next(10)));
    }

    internal static IReadOnlyCollection<IOrderItem> GetValidOrderItems(int amount)
    {
        return Enumerable.Range(0, amount).Select(x => new OrderItem(
            Guid.NewGuid(),
            Random.Shared.Next(150),
            Random.Shared.Next(150))).ToList();
    }

    public static SubmitOrder GetValidSubmitOrderCommand()
    {
        return new SubmitOrder(GetValidOrderItems(Random.Shared.Next(10)));
    }

    public static StockReserved GetValidStockReservedEvent(Guid orderId)
    {
        return new StockReserved(orderId);
    }

    public static PaymentProcessed GetValidPaymentProcessedEvent(Guid orderId)
    {
        return new PaymentProcessed(orderId);
    }

    public static OrderShipped GetValidOrderShippedEvent(Guid orderId)
    {
        return new OrderShipped(orderId);
    }

    public static CancelOrder GetValidCancelOrderCommand(Guid orderId)
    {
        return new CancelOrder(orderId);
    }

    public static CancelOrder GetCancelOrderCommand()
    {
        return new CancelOrder(NewId.NextGuid());
    }
}
