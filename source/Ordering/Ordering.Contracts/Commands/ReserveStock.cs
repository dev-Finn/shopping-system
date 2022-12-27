using MassTransit;
using Ordering.Contracts.Models;

namespace Ordering.Contracts.Commands;

[EntityName("reserve-stock")]
public sealed record ReserveStock(Order Order);
