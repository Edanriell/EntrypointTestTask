using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;

namespace Server.Application.Statistics.GetOrderStatusDistribution;

internal sealed class GetOrderStatusDistributionQueryHandler
    : IQueryHandler<GetOrderStatusDistributionQuery, GetOrderStatusDistributionResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetOrderStatusDistributionQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetOrderStatusDistributionResponse>> Handle(
        GetOrderStatusDistributionQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append(@"
            SELECT 
                status AS Status,
                COUNT(*) AS Count
            FROM orders o
            GROUP BY status
            ORDER BY status
        ");

        IEnumerable<(string Status, int Count)> distributionData =
            await connection.QueryAsync<(string Status, int Count)>(sqlBuilder.ToString());

        // Map string status -> OrderStatus enum
        var distributions = distributionData
            .Select(x => new OrderStatusDistribution
            {
                Status = Enum.Parse<OrderStatus>(x.Status, true),
                Count = x.Count
            })
            .ToList();

        return Result.Success(new GetOrderStatusDistributionResponse
        {
            OrderStatusDistributions = distributions
        });
    }
}
