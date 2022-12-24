using Ordering.Service.Models;

namespace Ordering.Service.Contracts.Commands.Incomming;

public sealed record CreateOrderCommand()
{
    public IReadOnlyCollection<Position> Positions { get; }
}
