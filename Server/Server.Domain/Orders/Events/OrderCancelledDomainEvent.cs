using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderCancelledDomainEvent(
    Guid OrderId,
    OrderStatus Status,
    CancellationReason cancellationReason) : IDomainEvent;
