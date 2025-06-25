using Server.Domain.Abstractions;

namespace Server.Domain.Products.Events;

public sealed record ProductBackInStockDomainEvent(Guid ProductId) : IDomainEvent;
