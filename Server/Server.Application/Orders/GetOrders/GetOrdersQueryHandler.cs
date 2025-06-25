using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Orders.GetOrders;

internal sealed class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, IReadOnlyList<OrdersResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetOrdersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<IReadOnlyList<OrdersResponse>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
                            SELECT
                                o.id AS Id,
                                o.client_id AS ClientId,
                                o.order_number AS OrderNumber,
                                o.status AS Status,
                                o.total_amount AS TotalAmount,
                                o.shipping_address AS ShippingAddress,
                                o.created_at AS CreatedAt,
                                o.confirmed_at AS ConfirmedAt,
                                o.shipped_at AS ShippedAt,
                                o.delivered_at AS DeliveredAt,
                                o.cancelled_at AS CancelledAt,
                                o.cancellation_reason AS CancellationReason,
                                o.return_reason AS ReturnReason,
                                o.refund_reason AS RefundReason,
                                o.tracking_number AS TrackingNumber,
                                
                                -- Client information
                                u.id AS ClientId,
                                u.first_name AS ClientFirstName,
                                u.last_name AS ClientLastName,
                                u.email AS ClientEmail,
                                u.phone_number AS ClientPhoneNumber,
                                
                                -- Payment information
                                p.id AS PaymentId,
                                p.total_amount AS PaymentTotalAmount,
                                p.payment_status AS PaymentStatus,
                                p.paid_amount AS PaidAmount,
                                p.outstanding_amount AS OutstandingAmount
                                
                            FROM orders o
                            INNER JOIN users u ON o.client_id = u.id
                            LEFT JOIN payments p ON o.id = p.order_id
                            ORDER BY o.created_at DESC
                           """;

        var orderDictionary = new Dictionary<Guid, OrdersResponse>();

        await connection.QueryAsync<OrdersResponse, ClientResponse, PaymentResponse, OrdersResponse>(
            sql,
            (order, client, payment) =>
            {
                if (!orderDictionary.TryGetValue(order.Id, out OrdersResponse? existingOrder))
                {
                    var newOrder = new OrdersResponse
                    {
                        Id = order.Id,
                        ClientId = order.ClientId,
                        OrderNumber = order.OrderNumber,
                        Status = order.Status,
                        TotalAmount = order.TotalAmount,
                        ShippingAddress = order.ShippingAddress,
                        CreatedAt = order.CreatedAt,
                        ConfirmedAt = order.ConfirmedAt,
                        ShippedAt = order.ShippedAt,
                        DeliveredAt = order.DeliveredAt,
                        CancelledAt = order.CancelledAt,
                        CancellationReason = order.CancellationReason,
                        ReturnReason = order.ReturnReason,
                        RefundReason = order.RefundReason,
                        TrackingNumber = order.TrackingNumber,
                        Client = client,
                        Payment = payment
                    };
                    orderDictionary.Add(order.Id, newOrder);
                    return newOrder;
                }

                return existingOrder;
            },
            splitOn: "ClientId,PaymentId"
        );

        return orderDictionary.Values.ToList();
    }
}
