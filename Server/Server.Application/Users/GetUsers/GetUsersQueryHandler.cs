using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Users.GetClients;

internal sealed class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, GetUsersResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetUsersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetUsersResponse>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
                           SELECT 
                               u.id as Id,
                               u.first_name as FirstName,
                               u.last_name as LastName,
                               u.email as Email,
                               u.phone_number as PhoneNumber,
                               u.gender as Gender,
                               u.address_country as Country,
                               u.address_city as City,
                               u.address_zipcode as ZipCode,
                               u.address_street as Street,
                               u.created_at as CreatedOnUtc,
                               COALESCE(ARRAY_AGG(DISTINCT r.name) FILTER (WHERE r.name IS NOT NULL), '{}') as Roles,
                               COALESCE(ARRAY_AGG(DISTINCT p.name) FILTER (WHERE p.name IS NOT NULL), '{}') as Permissions
                           FROM users u
                           LEFT JOIN role_user ru ON u.id = ru.users_id
                           LEFT JOIN roles r ON ru.roles_id = r.id
                           LEFT JOIN role_permissions rp ON r.id = rp.role_id
                           LEFT JOIN permissions p ON rp.permission_id = p.id
                           GROUP BY 
                               u.id,
                               u.first_name,
                               u.last_name,
                               u.email,
                               u.phone_number,
                               u.gender,
                               u.address_country,
                               u.address_city,
                               u.address_zipcode,
                               u.address_street,
                               u.created_at
                           ORDER BY u.created_at DESC
                           """;

        IEnumerable<User> users = await connection.QueryAsync<User>(sql);

        return new GetUsersResponse(users.ToList());
    }
}
