using MassTransit;

namespace Ordering.Contracts.Events;

[EntityName("order-cancelled")]
public sealed record OrderCancelled(Guid OrderId);
