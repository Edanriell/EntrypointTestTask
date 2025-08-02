using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Domain.Users.Events;

public sealed record UserAddressChangedDomainEvent(Guid UserId, Address OldAddress, Address NewAddress) : IDomainEvent;
 
