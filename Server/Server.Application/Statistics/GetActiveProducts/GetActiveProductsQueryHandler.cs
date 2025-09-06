using System.Data;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Statistics.GetActiveProducts;

internal sealed class GetActiveProductsQueryHandler : IQueryHandler<GetActiveProductsQuery, GetActiveProductsResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetActiveProductsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetActiveProductsResponse>> Handle(
        GetActiveProductsQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
        var sqlBuilder = new StringBuilder();

        sqlBuilder.Append("""
                              SELECT 
                                  COUNT(*) FILTER (WHERE status != 'Deleted') AS ActiveProducts,
                                  COUNT(*) FILTER (WHERE status != 'Deleted' AND (total_stock - reserved) <= 25) AS LowStockProducts
                              FROM products
                          """);

        (int ActiveProducts, int LowStockProducts) result =
            await connection.QuerySingleAsync<(int ActiveProducts, int LowStockProducts)>(
                sqlBuilder.ToString());

        return new GetActiveProductsResponse
        {
            ActiveProducts = result.ActiveProducts,
            LowStockProducts = result.LowStockProducts
        };
    }
}
