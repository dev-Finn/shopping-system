using MassTransit;

namespace Warehouse.Contracts.Events;

[EntityName("order-shipped")]
public sealed record OrderShipped(Guid OrderId);
