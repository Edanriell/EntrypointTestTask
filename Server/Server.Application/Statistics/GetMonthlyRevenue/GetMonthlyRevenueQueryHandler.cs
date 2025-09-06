using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Statistics.GetMonthlyRevenue;

internal sealed class GetMonthlyRevenueQueryHandler : IQueryHandler<GetMonthlyRevenueQuery, GetMonthlyRevenueResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetMonthlyRevenueQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetMonthlyRevenueResponse>> Handle(
        GetMonthlyRevenueQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append(@"
            SELECT
                COALESCE(SUM(o.total_amount), 0) AS total_revenue,  -- All-time revenue
                COALESCE(SUM(o.total_amount) FILTER (
                    WHERE o.created_at >= date_trunc('month', CURRENT_DATE)
                      AND o.created_at < date_trunc('month', CURRENT_DATE + interval '1 month')
                ), 0) AS this_month_revenue,
                COALESCE(SUM(o.total_amount) FILTER (
                    WHERE o.created_at >= date_trunc('month', CURRENT_DATE - interval '1 month')
                      AND o.created_at < date_trunc('month', CURRENT_DATE)
                ), 0) AS last_month_revenue
            FROM orders o
            WHERE o.status IN ('Completed', 'Delivered');
        ");

        (decimal TotalRevenue, decimal ThisMonthRevenue, decimal LastMonthRevenue) revenue =
            await connection
                .QuerySingleAsync<(decimal TotalRevenue, decimal ThisMonthRevenue, decimal LastMonthRevenue)>(
                    sqlBuilder.ToString());

        // Calculate month-over-month percentage
        double changePercent = 0;

        if (revenue.LastMonthRevenue == 0)
        {
            changePercent = revenue.ThisMonthRevenue > 0 ? 100 : 0; // New revenue this month
        }
        else
        {
            changePercent = (double)(revenue.ThisMonthRevenue - revenue.LastMonthRevenue) /
                (double)revenue.LastMonthRevenue * 100;
        }

        return Result.Success(new GetMonthlyRevenueResponse
        {
            TotalRevenue = revenue.TotalRevenue, // All-time
            ChangePercent = changePercent // Can be negative
        });
    }
}
