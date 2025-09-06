using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Products.Events;

public sealed record ProductStockReservedDomainEvent(Guid ProductId, Quantity ReservedQuantity) : IDomainEvent;
 
