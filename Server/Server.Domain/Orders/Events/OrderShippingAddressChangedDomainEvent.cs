using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Orders.Events;

public sealed record
    OrderShippingAddressChangedDomainEvent(Guid OrderId, Address OldAddress, Address NewAddress) : IDomainEvent;
 
