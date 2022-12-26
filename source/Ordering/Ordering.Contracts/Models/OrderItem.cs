namespace Ordering.Contracts.Models;

public sealed record OrderItem(Guid ProductId, decimal Price, decimal Amount);
