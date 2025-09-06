using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderReturnedDomainEvent(Guid OrderId, Guid ClientId, ReturnReason Reason) : IDomainEvent;
 
 
