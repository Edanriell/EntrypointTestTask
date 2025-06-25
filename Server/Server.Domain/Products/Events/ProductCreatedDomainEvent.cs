using Server.Domain.Abstractions;

namespace Server.Domain.Products.Events;

public sealed record ProductCreatedDomainEvent(Guid ProductId) : IDomainEvent;
