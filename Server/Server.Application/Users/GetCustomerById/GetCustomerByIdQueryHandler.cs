using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Users;

namespace Server.Application.Users.GetCustomerById;

internal sealed class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, GetCustomerByIdResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCustomerByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetCustomerByIdResponse>> Handle(
        GetCustomerByIdQuery request,
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
                               COUNT(DISTINCT o.id) as TotalOrders,
                               COALESCE(SUM(CASE WHEN o.status = 'Completed' THEN 1 ELSE 0 END), 0) as CompletedOrders,
                               COALESCE(SUM(CASE WHEN o.status = 'Pending' THEN 1 ELSE 0 END), 0) as PendingOrders,
                               COALESCE(SUM(CASE WHEN o.status = 'Cancelled' THEN 1 ELSE 0 END), 0) as CancelledOrders,
                               COALESCE(SUM(op.total_price_amount), 0) as TotalSpent,
                               MAX(o.created_at) as LastOrderDate
                           FROM users u
                           INNER JOIN role_user ur ON u.id = ur.users_id
                           INNER JOIN roles r ON ur.roles_id = r.id
                           LEFT JOIN orders o ON u.id = o.client_id
                           LEFT JOIN order_products op ON o.id = op.order_id
                           WHERE r.name = 'Customer'
                           AND u.id = @UserId
                           GROUP BY u.id, u.first_name, u.last_name, u.email, u.phone_number, 
                                    u.gender, u.address_country, u.address_city, u.address_zipcode, 
                                    u.address_street, u.created_at
                           """;

        GetCustomerByIdResponse? customer = await connection.QueryFirstOrDefaultAsync<GetCustomerByIdResponse>(sql, new
        {
            request.UserId
        });

        if (customer is null)
        {
            return Result.Failure<GetCustomerByIdResponse>(UserErrors.NotFound);
        }

        await LoadRecentOrders(connection, customer);

        return Result.Success(customer);
    }

    private async Task LoadRecentOrders(IDbConnection connection, GetCustomerByIdResponse getCustomerById)
    {
        var parameters = new DynamicParameters();
        parameters.Add("CustomerId", getCustomerById.Id);

        const string recentOrdersSql = """
                                       SELECT 
                                           o.client_id as UserId,
                                           o.id as OrderId,
                                           COALESCE(SUM(op.total_price_amount), 0) as TotalAmount,
                                           o.status as Status,
                                           o.created_at as CreatedOnUtc
                                       FROM orders o
                                       LEFT JOIN order_products op ON o.id = op.order_id
                                       WHERE o.client_id = @CustomerId
                                       AND o.created_at >= CURRENT_DATE - INTERVAL '30 days'
                                       GROUP BY o.client_id, o.id, o.status, o.created_at
                                       ORDER BY o.created_at DESC
                                       LIMIT 5
                                       """;

        IEnumerable<RecentOrder> recentOrders = await connection.QueryAsync<RecentOrder>(recentOrdersSql, parameters);

        getCustomerById.RecentOrders = recentOrders.ToList();
    }
}
