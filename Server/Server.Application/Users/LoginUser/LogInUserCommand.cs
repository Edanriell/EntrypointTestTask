using Server.Application.Abstractions.Messaging;

namespace Server.Application.Users.LoginUser;

public sealed record LogInUserCommand(string Email, string Password)
    : ICommand<AccessTokenResponse>;
