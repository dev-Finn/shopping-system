using System.Runtime.CompilerServices;
using MassTransit;
using Warehouse.Contracts.Events;

namespace Warehouse.Contracts;

public static class WarehouseCorrelation
{
    [ModuleInitializer]
    public static void Initialize()
    {
        MessageCorrelation.UseCorrelationId<OrderShipped>(x => x.OrderId);
        MessageCorrelation.UseCorrelationId<StockReserved>(x => x.OrderId);
    }
}
