using Ordering.Contracts.Models;
using Payment.UnitTests.Contracts;

namespace Payment.UnitTests;

public static class TestData
{
    internal static IReadOnlyCollection<IOrderItem> GetValidOrderItems(int amount)
    {
        return Enumerable.Range(0, amount).Select(x => new OrderItem(
            Guid.NewGuid(),
            Random.Shared.Next(150),
            Random.Shared.Next(150))).ToList();
    }
}
