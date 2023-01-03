using MassTransit;
using Ordering.Contracts.Models;

namespace Ordering.Contracts.Commands;

[EntityName("submit-order")]
public sealed record SubmitOrder(IReadOnlyCollection<IOrderItem> Positions);
