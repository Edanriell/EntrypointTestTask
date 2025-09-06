using Server.Domain.Abstractions;

namespace Server.Domain.Products.Events;

public sealed record ProductDeletedDomainEvent(Guid ProductId) : IDomainEvent;
  
