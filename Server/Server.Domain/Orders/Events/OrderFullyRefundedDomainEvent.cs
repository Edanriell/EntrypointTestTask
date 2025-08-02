using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Orders.Events;

public sealed record OrderFullyRefundedDomainEvent(
    Guid OrderId,
    Guid ClientId,
    Money TotalRefundedAmount) : IDomainEvent;
