using MassTransit;
using Ordering.Contracts.Models;

namespace Ordering.Contracts.Commands;

[EntityName("process-payment")]
public sealed record ProcessPayment(Guid OrderId, IReadOnlyCollection<IOrderItem> Items);
