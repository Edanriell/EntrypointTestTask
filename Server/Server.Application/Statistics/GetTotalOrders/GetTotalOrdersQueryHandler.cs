using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Statistics.GetTotalOrders;

internal sealed class GetTotalOrdersQueryHandler : IQueryHandler<GetTotalOrdersQuery, GetTotalOrdersResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetTotalOrdersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetTotalOrdersResponse>> Handle(
        GetTotalOrdersQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append(@"
            SELECT
                COUNT(*) AS total_orders,
                COUNT(*) FILTER (
                    WHERE EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM CURRENT_DATE)
                      AND EXTRACT(MONTH FROM created_at) = EXTRACT(MONTH FROM CURRENT_DATE)
                ) AS this_month_orders,
                COUNT(*) FILTER (
                    WHERE EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM CURRENT_DATE - INTERVAL '1 month')
                      AND EXTRACT(MONTH FROM created_at) = EXTRACT(MONTH FROM CURRENT_DATE - INTERVAL '1 month')
                ) AS last_month_orders
            FROM orders;
        ");

        (int TotalOrders, int ThisMonthOrders, int LastMonthOrders) result =
            await connection.QuerySingleAsync<(int TotalOrders, int ThisMonthOrders, int LastMonthOrders)>(
                sqlBuilder.ToString());

        // Calculate percentage change
        double changePercent = 0;
        if (result.LastMonthOrders == 0 && result.ThisMonthOrders > 0)
        {
            changePercent = 100;
        }
        else if (result.LastMonthOrders > 0)
        {
            changePercent = (double)(result.ThisMonthOrders - result.LastMonthOrders) / result.LastMonthOrders * 100;
        }

        return new GetTotalOrdersResponse
        {
            TotalOrders = result.TotalOrders,
            ChangePercent = changePercent
        };
    }
}
