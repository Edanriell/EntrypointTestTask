using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Products;

namespace Server.Application.Products.GetProductById;

internal sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetProductByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<ProductResponse>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
                            SELECT
                                id AS Id,
                                name As Name,
                                description AS Description,
                                price_amount AS Price,
                                reserved AS Reserved,
                                stock AS Stock,
                                status AS Status,
                                created_at AS CreatedAt,
                                last_updated_at AS LastUpdatedAt,
                                last_restocked_at AS LastRestockedAt
                                FROM products
                                WHERE id = @ProductId
                           """;

        ProductResponse?
            product = await connection.QueryFirstOrDefaultAsync(
                sql,
                new { request.ProductId });

        if (product is null)
        {
            return Result.Failure<ProductResponse>(ProductErrors.NotFound);
        }

        return product;
    }
}
