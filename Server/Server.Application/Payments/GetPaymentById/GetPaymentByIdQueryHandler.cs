using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Payments;

namespace Server.Application.Payments.GetPaymentById;

internal sealed class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, PaymentResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetPaymentByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<PaymentResponse>> Handle(
        GetPaymentByIdQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
                           SELECT
                               p.id AS Id,
                               p.order_id AS OrderId,
                               p.total_amount AS TotalAmount,
                               p.payment_status AS PaymentStatus,
                               p.paid_amount AS PaidAmount,
                               p.outstanding_amount AS OutstandingAmount,
                               p.payment_completed_at AS PaymentCompletedAt,
                               p.created_on_utc AS CreatedOnUtc,
                               p.modified_on_utc AS ModifiedOnUtc
                           FROM payments p
                           WHERE p.id = @PaymentId
                           """;

        PaymentResponse? payment = await connection.QuerySingleOrDefaultAsync<PaymentResponse>(
            sql,
            new { request.PaymentId });

        if (payment is null)
        {
            return Result.Failure<PaymentResponse>(PaymentErrors.PaymentNotFound);
        }

        return Result.Success(payment);
    }
}
