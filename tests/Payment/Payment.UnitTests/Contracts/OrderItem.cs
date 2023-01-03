using Ordering.Contracts.Models;

namespace Payment.UnitTests.Contracts;

internal sealed record OrderItem(Guid ProductId, decimal Price, decimal Amount) : IOrderItem;
