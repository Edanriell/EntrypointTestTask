using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Payments.GetPaymentsByOrderId;

internal sealed class GetPaymentsByOrderIdQueryHandler
    : IQueryHandler<GetPaymentsByOrderIdQuery, IReadOnlyList<GetPaymentsByOrderIdResponse>>

{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetPaymentsByOrderIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<GetPaymentsByOrderIdResponse>>> Handle(
        GetPaymentsByOrderIdQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
                           SELECT 
                               p.Id,
                               p.OrderId,
                               p.Amount,
                               p.Currency,
                               p.PaymentStatus,
                               p.PaymentMethod,
                               p.PaymentReference,
                               p.CreatedAt,
                               p.PaymentCompletedAt
                           FROM Payments p
                           WHERE p.OrderId = @OrderId
                           ORDER BY p.CreatedAt DESC
                           """;

        IEnumerable<GetPaymentsByOrderIdResponse> payments = await connection.QueryAsync<GetPaymentsByOrderIdResponse>(
            sql,
            new { request.OrderId });

        return Result.Success<IReadOnlyList<GetPaymentsByOrderIdResponse>>(payments.ToList());
    }
}
