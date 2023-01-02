namespace Ordering.Contracts.Models;

public sealed record Order(Guid OrderId, IEnumerable<OrderItem> Items);
