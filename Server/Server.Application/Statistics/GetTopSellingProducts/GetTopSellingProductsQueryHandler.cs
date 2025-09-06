using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Statistics.GetTopSellingProducts;

internal sealed class GetTopSellingProductsQueryHandler
    : IQueryHandler<GetTopSellingProductsQuery, GetTopSellingProductsResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetTopSellingProductsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetTopSellingProductsResponse>> Handle(
        GetTopSellingProductsQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append(@"
            SELECT 
                op.product_name AS ProductName,
                SUM(op.quantity) AS UnitsSold,
                COALESCE(SUM(op.total_price_amount), 0) AS Revenue
            FROM order_products op
            INNER JOIN products p ON p.id = op.product_id
            INNER JOIN orders o ON o.id = op.order_id
            WHERE o.status NOT IN ('Cancelled', 'Returned')
            GROUP BY op.product_name
            ORDER BY UnitsSold DESC
            LIMIT 10
        ");

        IEnumerable<BestSellingProduct> products =
            await connection.QueryAsync<BestSellingProduct>(sqlBuilder.ToString());

        return Result.Success(new GetTopSellingProductsResponse
        {
            BestSellingProducts = products.ToList()
        });
    }
}
