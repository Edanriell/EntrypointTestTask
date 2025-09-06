using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Payments.GetPaymentsByOrderId;

internal sealed class GetPaymentsByOrderIdQueryHandler
    : IQueryHandler<GetPaymentsByOrderIdQuery, GetPaymentsByOrderIdResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetPaymentsByOrderIdQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetPaymentsByOrderIdResponse>> Handle(
        GetPaymentsByOrderIdQuery request,
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
                           WHERE p.order_id = @OrderId
                           ORDER BY p.created_at DESC
                           """;

        var paymentDictionary = new Dictionary<Guid, PaymentResponse>();

        await connection.QueryAsync<PaymentResponse, RefundResponse, PaymentResponse>(
            sql,
            (payment, refund) =>
            {
                if (!paymentDictionary.TryGetValue(payment.Id, out PaymentResponse? existingPayment))
                {
                    PaymentResponse newPayment = payment with
                    {
                        Refund = refund
                    };
                    paymentDictionary.Add(payment.Id, newPayment);
                    return newPayment;
                }

                return existingPayment;
            },
            new { request.OrderId },
            splitOn: "RefundId"
        );

        var payments = paymentDictionary.Values.ToList();

        var response = new GetPaymentsByOrderIdResponse
        {
            OrderId = request.OrderId,
            Payments = payments
        };

        return response;
    }
}
