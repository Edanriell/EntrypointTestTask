using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderProcessingStartedDomainEvent(Guid OrderId) : IDomainEvent;
 
