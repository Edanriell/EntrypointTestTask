using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Products;

namespace Server.Application.Products.GetProductById;

internal sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, GetProductByIdResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetProductByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetProductByIdResponse>> Handle(
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
                                price_currency as Currency,
                                total_stock AS TotalStock,
                                reserved AS Reserved,
                                (total_stock - reserved) as Available,
                                (total_stock - reserved = 0) as IsOutOfStock,
                                (reserved > 0) as HasReservations,
                                (total_stock - reserved > 0) as IsInStock,
                                status AS Status,
                                created_at AS CreatedAt,
                                last_updated_at AS LastUpdatedAt,
                                last_restocked_at AS LastRestockedAt
                                FROM products
                                WHERE id = @ProductId
                                AND status != 'Deleted'
                           """;

        GetProductByIdResponse?
            product = await connection.QueryFirstOrDefaultAsync<GetProductByIdResponse>(
                sql,
                new { request.ProductId });

        if (product is null)
        {
            return Result.Failure<GetProductByIdResponse>(ProductErrors.NotFound);
        }

        return product;
    }
}
