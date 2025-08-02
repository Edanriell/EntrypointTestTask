using Server.Domain.Abstractions;

namespace Server.Domain.Orders.Events;

public sealed record OrderReadyForShipmentDomainEvent(Guid OrderId) : IDomainEvent;
  
