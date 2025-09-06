using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderFailedDomainEvent(
    Guid OrderId,
    string Reason) : IDomainEvent;
 
