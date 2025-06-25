using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Products.GetProducts;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductsResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetProductsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<ProductsResponse>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
                            SELECT
                                id AS Id,
                                name As Name,
                                description AS Description,
                                price_amount AS Price,
                                stock AS Stock,
                                status AS Status,
                                created_at AS CreatedAt,
                                last_updated_at AS LastUpdatedAt,
                                last_restocked_at AS LastRestockedAt
                            FROM products
                           """;

        IEnumerable<ProductsResponse> products = await connection.QueryAsync<ProductsResponse>(sql);

        return products.ToList();
    }
}
