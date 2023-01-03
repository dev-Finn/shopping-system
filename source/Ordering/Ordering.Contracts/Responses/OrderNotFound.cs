using MassTransit;

namespace Ordering.Contracts.Responses;

[EntityName("order-cancelled")]
public sealed record OrderNotFound(Guid OrderId);
