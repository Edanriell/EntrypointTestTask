using Server.Domain.Abstractions;

namespace Server.Domain.Users.Events;

public sealed record UserRoleRemovedDomainEvent(Guid UserId, int RoleId) : IDomainEvent;
