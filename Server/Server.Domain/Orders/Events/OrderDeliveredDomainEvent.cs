using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderDeliveredDomainEvent(Guid OrderId, Guid ClientId) : IDomainEvent;
 
 
