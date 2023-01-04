using System.Runtime.CompilerServices;
using MassTransit;
using Warehouse.Contracts.Events;

namespace Warehouse.Contracts;

public static class WarehouseCorrelation
{
    [ModuleInitializer]
    public static void Initialize()
    {
        LogContext.Info?.Log("Initializing Warehouse Message Correlation...");

        MessageCorrelation.UseCorrelationId<OrderShipped>(x => x.OrderId);
        MessageCorrelation.UseCorrelationId<StockReserved>(x => x.OrderId);

        LogContext.Info?.Log("Warehouse Payment Correlation Initialized!");
    }
}
