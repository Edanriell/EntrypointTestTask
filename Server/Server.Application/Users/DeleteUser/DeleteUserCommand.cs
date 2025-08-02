using Server.Application.Abstractions.Messaging;

namespace Server.Application.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : ICommand;
 
 
