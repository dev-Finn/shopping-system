namespace Ordering.Contracts.Commands;

public sealed record CancelOrderCommand(Guid OrderId);
