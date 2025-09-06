using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Application.Statistics.GetCustomerGrowthAndOrderVolume;

internal sealed class GetCustomerGrowthAndOrderVolumeQueryHandler
    : IQueryHandler<GetCustomerGrowthAndOrderVolumeQuery, GetCustomerGrowthAndOrderVolumeResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCustomerGrowthAndOrderVolumeQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetCustomerGrowthAndOrderVolumeResponse>> Handle(
        GetCustomerGrowthAndOrderVolumeQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append(@"
        SELECT
            EXTRACT(MONTH FROM (o.created_at AT TIME ZONE 'UTC')) AS MonthNumber,
            COUNT(DISTINCT o.client_id) AS TotalCustomers,
            COUNT(o.id) AS TotalOrders
        FROM orders o
        WHERE EXTRACT(YEAR FROM (o.created_at AT TIME ZONE 'UTC')) = EXTRACT(YEAR FROM (CURRENT_DATE AT TIME ZONE 'UTC'))
        GROUP BY MonthNumber
        ORDER BY MonthNumber;
        ");

        IEnumerable<(int MonthNumber, int TotalCustomers, int TotalOrders)> growthData =
            await connection.QueryAsync<(int MonthNumber, int TotalCustomers, int TotalOrders)>(
                sqlBuilder.ToString());

        var trend = growthData
            .Select(x => new CustomerGrowthOrders
            {
                Month = (Month)(x.MonthNumber - 1), // 1 → Jan, 12 → Dec
                TotalCustomers = x.TotalCustomers,
                TotalOrders = x.TotalOrders
            })
            .ToList();

        return Result.Success(new GetCustomerGrowthAndOrderVolumeResponse
        {
            Trend = trend
        });
    }
}
