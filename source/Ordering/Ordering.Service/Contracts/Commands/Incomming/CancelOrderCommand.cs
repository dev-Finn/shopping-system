namespace Ordering.Service.Contracts.Commands.Incomming;

public sealed record CancelOrderCommand(Guid OrderId, DateTime TimeStampUtc);
