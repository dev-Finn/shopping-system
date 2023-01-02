using MassTransit;
using Ordering.Contracts.Models;

namespace Ordering.Contracts.Commands;

[EntityName("ship-order")]
public sealed record ShipOrder(Guid OrderId, IEnumerable<OrderItem> Items);
