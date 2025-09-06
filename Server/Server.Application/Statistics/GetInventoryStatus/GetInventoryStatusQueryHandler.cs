using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Shared;

namespace Server.Application.Statistics.GetInventoryStatus;

internal sealed class GetInventoryStatusQueryHandler
    : IQueryHandler<GetInventoryStatusQuery, GetInventoryStatusResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetInventoryStatusQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetInventoryStatusResponse>> Handle(
        GetInventoryStatusQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append(@"
            SELECT 
                CASE
                    WHEN (p.total_stock - p.reserved) <= 0 THEN 'OutOfStock'
                    WHEN (p.total_stock - p.reserved) <= 25 THEN 'LowStock'
                    ELSE 'InStock'
                END AS InventoryStatus,
                COUNT(*) AS Count
            FROM products p
            WHERE p.status != 'Deleted'
            GROUP BY InventoryStatus
        ");

        IEnumerable<(string InventoryStatus, int Count)> statusData =
            await connection.QueryAsync<(string InventoryStatus, int Count)>(sqlBuilder.ToString());

        var summaries = statusData
            .Select(x => new ProductInventory
            {
                InventoryStatus = Enum.Parse<InventoryStatus>(x.InventoryStatus, true),
                Count = x.Count
            })
            .ToList();

        return new GetInventoryStatusResponse
        {
            InventorySummary = summaries
        };
    }
}
