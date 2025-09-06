using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Payments;

namespace Server.Application.Payments.GetPaymentById;

internal sealed class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, GetPaymentByIdResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetPaymentByIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetPaymentByIdResponse>> Handle(
        GetPaymentByIdQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
                           SELECT 
                               p.id AS Id,
                               p.order_id AS OrderId,
                               p.amount AS Amount,
                               p.currency AS Currency,
                               p.payment_status AS PaymentStatus,
                               p.payment_method AS PaymentMethod,
                               p.created_at AS CreatedAt,
                               p.payment_completed_at AS PaymentCompletedAt,
                               p.payment_failed_at AS PaymentFailedAt,
                               p.payment_expired_at AS PaymentExpiredAt,
                               p.payment_failure_reason AS PaymentFailureReason,
                               
                               -- Single refund data (if exists)
                               r.id AS RefundId,
                               r.amount AS RefundAmount,
                               r.currency AS RefundCurrency,
                               r.refund_reason AS RefundReason,
                               r.created_at AS RefundCreatedAt,
                               r.processed_at AS RefundProcessedAt

                           FROM payments p
                           LEFT JOIN refunds r ON p.id = r.payment_id
                           WHERE p.id = @PaymentId
                           """;

        IEnumerable<GetPaymentByIdResponse> results =
            await connection.QueryAsync<GetPaymentByIdResponse, RefundResponse, GetPaymentByIdResponse>(
                sql,
                (payment, refund) =>
                {
                    return payment with
                    {
                        Refund = refund
                    };
                },
                new { request.PaymentId },
                splitOn: "RefundId"
            );

        GetPaymentByIdResponse? result = results.FirstOrDefault();

        if (result is null)
        {
            return Result.Failure<GetPaymentByIdResponse>(PaymentErrors.NotFound);
        }

        return result;
    }
}
