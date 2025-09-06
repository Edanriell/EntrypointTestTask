using Server.Application.Abstractions.Messaging;

namespace Server.Application.Users.GetLoggedInUser;

public sealed record GetLoggedInUserQuery : IQuery<GetLoggedInUserResponse>;
 
