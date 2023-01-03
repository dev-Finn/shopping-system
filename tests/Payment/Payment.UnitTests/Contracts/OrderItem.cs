using Ordering.Contracts.Models;

namespace Payment.UnitTests.Contracts;

public sealed record OrderItem(Guid ProductId, decimal Price, decimal Amount) : IOrderItem;
