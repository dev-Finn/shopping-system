namespace Ordering.Service.Models;

public sealed record Position(Guid ProductId, decimal Price, decimal Amount);
