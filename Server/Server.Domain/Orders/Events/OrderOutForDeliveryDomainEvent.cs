using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderOutForDeliveryDomainEvent(Guid OrderId) : IDomainEvent;
 
 
