using Ordering.Contracts.Models;

namespace Warehouse.UnitTests.Contracts;

internal sealed record OrderItem(Guid ProductId, decimal Price, decimal Amount) : IOrderItem;
