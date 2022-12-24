namespace Ordering.Service.Contracts.Commands.Outgoing;

public sealed record ShipOrderCommand(Guid OrderId);
