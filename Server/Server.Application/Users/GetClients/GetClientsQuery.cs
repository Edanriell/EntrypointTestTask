using Server.Application.Abstractions.Messaging;

namespace Server.Application.Users.GetClients;

public sealed record GetClientsQuery : IQuery<IReadOnlyList<GetClientsResponse>>;
