using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Application.Statistics.GetOrdersAndRevenueTrend;

internal sealed class GetOrdersRevenueTrendQueryHandler
    : IQueryHandler<GetOrdersAndRevenueTrendQuery, GetOrdersAndRevenueTrendResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetOrdersRevenueTrendQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetOrdersAndRevenueTrendResponse>> Handle(
        GetOrdersAndRevenueTrendQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append(@"
            SELECT
                EXTRACT(MONTH FROM o.created_at) AS MonthNumber,
                COUNT(*) AS Orders,
                COALESCE(SUM(o.total_amount), 0) AS Revenue
            FROM orders o
            WHERE o.created_at >= date_trunc('month', CURRENT_DATE) - INTERVAL '11 months'
            GROUP BY MonthNumber
            ORDER BY MonthNumber
        ");

        IEnumerable<(int MonthNumber, int Orders, int Revenue)> trendData =
            await connection.QueryAsync<(int MonthNumber, int Orders, int Revenue)>(sqlBuilder.ToString());

        var trend = trendData
            .Select(x => new MonthlyOrdersRevenue
            {
                Month = (Month)(x.MonthNumber - 1), // Enum is 0-based
                Orders = x.Orders,
                Revenue = x.Revenue
            })
            .ToList();

        return new GetOrdersAndRevenueTrendResponse
        {
            Trend = trend
        };
    }
}
