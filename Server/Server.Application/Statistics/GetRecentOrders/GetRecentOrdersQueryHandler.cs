using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Statistics.GetRecentOrders;

internal sealed class GetRecentOrdersQueryHandler : IQueryHandler<GetRecentOrdersQuery, GetRecentOrdersResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetRecentOrdersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetRecentOrdersResponse>> Handle(
        GetRecentOrdersQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append(@"
            SELECT 
                o.order_number AS Number,
                CONCAT(u.first_name, ' ', u.last_name) AS Customer,
                o.status AS Status,
                o.total_amount AS Total,
                o.created_at AS Date
            FROM orders o
            INNER JOIN users u ON o.client_id = u.id
            WHERE o.status IN ('Completed', 'Delivered', 'InTransit')
            ORDER BY o.created_at DESC
            LIMIT 10
        ");


        IEnumerable<RecentOrder> orders = await connection.QueryAsync<RecentOrder>(sqlBuilder.ToString());

        return Result.Success(new GetRecentOrdersResponse
        {
            RecentOrders = orders.ToList()
        });
    }
}
