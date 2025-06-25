using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Products.Events;

public sealed record ProductPriceChangedDomainEvent(Guid ProductId, Money oldPrice, Money price) : IDomainEvent;
