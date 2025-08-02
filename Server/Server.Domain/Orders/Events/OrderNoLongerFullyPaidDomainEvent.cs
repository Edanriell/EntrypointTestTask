using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderNoLongerFullyPaidDomainEvent(
    Guid OrderId,
    Guid ClientId
) : IDomainEvent;
