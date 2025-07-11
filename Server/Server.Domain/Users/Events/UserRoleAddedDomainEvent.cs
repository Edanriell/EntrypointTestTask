using Server.Domain.Abstractions;

namespace Server.Domain.Users.Events;

public sealed record UserRoleAddedDomainEvent(Guid UserId, int RoleId) : IDomainEvent;
