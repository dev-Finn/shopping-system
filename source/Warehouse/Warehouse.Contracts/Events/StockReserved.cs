using MassTransit;

namespace Warehouse.Contracts.Events;

[EntityName("stock-reserved")]
public sealed record StockReserved(Guid OrderId);
