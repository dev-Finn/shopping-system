using Ordering.Contracts.Models;

namespace Ordering.UnitTests.Contracts;

internal sealed record OrderItem(Guid ProductId, decimal Price, decimal Amount) : IOrderItem;
