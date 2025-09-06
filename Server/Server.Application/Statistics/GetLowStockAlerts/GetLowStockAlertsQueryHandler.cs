using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Statistics.GetLowStockAlerts;

internal sealed class GetLowStockAlertsQueryHandler
    : IQueryHandler<GetLowStockAlertsQuery, GetLowStockAlertsResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetLowStockAlertsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetLowStockAlertsResponse>> Handle(
        GetLowStockAlertsQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append(@"
            SELECT 
                p.name AS ProductName,
                (p.total_stock - p.reserved) AS UnitsInStock
            FROM products p
            WHERE (p.total_stock - p.reserved) <= 25
              AND p.status != 'Deleted'
            ORDER BY UnitsInStock ASC
        ");

        IEnumerable<LowStockProduct> products = await connection.QueryAsync<LowStockProduct>(sqlBuilder.ToString());

        return Result.Success(new GetLowStockAlertsResponse
        {
            LowStockProducts = products.ToList()
        });
    }
}
