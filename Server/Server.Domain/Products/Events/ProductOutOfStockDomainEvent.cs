using Server.Domain.Abstractions;

namespace Server.Domain.Products.Events;

public sealed record ProductOutOfStockDomainEvent(Guid ProductId) : IDomainEvent;
 
 
