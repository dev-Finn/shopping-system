using MassTransit;

namespace Ordering.Contracts.Events;

[EntityName("order-processed")]
public sealed record OrderProcessed(Guid OrderId);
