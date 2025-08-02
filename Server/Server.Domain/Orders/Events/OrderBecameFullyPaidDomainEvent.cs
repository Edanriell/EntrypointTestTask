using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderBecameFullyPaidDomainEvent(
    Guid OrderId,
    Guid ClientId
) : IDomainEvent;
