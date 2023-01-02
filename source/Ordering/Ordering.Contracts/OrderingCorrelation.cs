using System.Runtime.CompilerServices;
using MassTransit;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;

namespace Ordering.Contracts;

public static class OrderingCorrelation
{
    [ModuleInitializer]
    public static void Initialize()
    {
        MessageCorrelation.UseCorrelationId<SubmitOrder>(x => NewId.NextGuid());
        MessageCorrelation.UseCorrelationId<ProcessPayment>(x => x.OrderId);
        MessageCorrelation.UseCorrelationId<ReserveStock>(x => x.OrderId);
        MessageCorrelation.UseCorrelationId<ShipOrder>(x => x.OrderId);
        MessageCorrelation.UseCorrelationId<CancelOrder>(x => x.OrderId);

        MessageCorrelation.UseCorrelationId<OrderSubmitted>(x => x.OrderId);
        MessageCorrelation.UseCorrelationId<OrderCancelled>(x => x.OrderId);
        MessageCorrelation.UseCorrelationId<OrderProcessed>(x => x.OrderId);
    }
}
