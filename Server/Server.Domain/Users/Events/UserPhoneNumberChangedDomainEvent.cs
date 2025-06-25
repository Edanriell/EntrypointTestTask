using Server.Domain.Abstractions;

namespace Server.Domain.Users.Events;

public sealed record UserPhoneNumberChangedDomainEvent(
    Guid UserId,
    PhoneNumber OldPhoneNumber,
    PhoneNumber NewPhoneNumber) : IDomainEvent;
