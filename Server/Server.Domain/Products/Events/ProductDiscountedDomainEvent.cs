using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Products.Events;

public sealed record ProductDiscountedDomainEvent(Guid ProductId, Money price, Money oldPrice) : IDomainEvent;
 
