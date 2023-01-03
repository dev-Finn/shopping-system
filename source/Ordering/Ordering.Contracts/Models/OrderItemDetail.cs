namespace Ordering.Contracts.Models;

public interface IOrderItem
{
    Guid ProductId { get; }
    decimal Price { get; }
    decimal Amount { get; }
}
