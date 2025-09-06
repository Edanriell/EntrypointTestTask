using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderCreatedDomainEvent(Guid OrderId, Guid ClientId) : IDomainEvent;
 
 
