using Server.Application.Abstractions.Messaging;

namespace Server.Application.Users.ChangeUserPassword;
 
public sealed record ChangeUserPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : ICommand;
