﻿using Server.Domain.Users;

namespace Server.Api.Controllers.Users;

public sealed record UpdateUserRequest(
    Guid UserId,
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber,
    Gender? Gender,
    string? Country,
    string? City,
    string? ZipCode,
    string? Street);
 
 
