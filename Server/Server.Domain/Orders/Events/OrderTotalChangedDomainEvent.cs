using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Orders.Events;

public sealed record OrderTotalChangedDomainEvent(
    Guid OrderId,
    Money NewTotal
) : IDomainEvent;
 
 
