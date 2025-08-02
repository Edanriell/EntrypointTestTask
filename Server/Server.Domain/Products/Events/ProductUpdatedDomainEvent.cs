using Server.Domain.Abstractions;

namespace Server.Domain.Products.Events;

public sealed record ProductUpdatedDomainEvent(Guid ProductId) : IDomainEvent;
 
