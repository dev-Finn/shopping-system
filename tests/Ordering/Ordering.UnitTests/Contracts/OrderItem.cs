using Ordering.Contracts.Models;

namespace Ordering.UnitTests.Contracts;

public sealed record OrderItem(Guid ProductId, decimal Price, decimal Amount) : IOrderItem;
