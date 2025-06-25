using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Users.GetClients;

internal sealed class GetClientsQueryHandler : IQueryHandler<GetClientsQuery, IReadOnlyList<GetClientsResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetClientsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<GetClientsResponse>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
                           SELECT 
                               id as Id,
                               first_name as FirstName,
                               last_name as LastName,
                               email as Email,
                               phone_number as PhoneNumber,
                               created_on_utc as CreatedOnUtc
                           FROM users
                           """;

        IEnumerable<GetClientsResponse> clients = await connection.QueryAsync<GetClientsResponse>(sql);

        return clients.ToList();
    }
}
