using MassTransit;
using Ordering.Contracts.Models;

namespace Ordering.Contracts.Events;

[EntityName("order-submitted")]
public sealed record OrderSubmitted(Guid OrderId, IReadOnlyCollection<OrderItem> Items);
