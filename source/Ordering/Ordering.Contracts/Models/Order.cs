namespace Ordering.Contracts.Models;

public sealed record Order(Guid OrderId, IReadOnlyCollection<OrderItem> Items);
