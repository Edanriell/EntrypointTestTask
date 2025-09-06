using System.Data;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;

namespace Server.Application.Orders.GetOrderByNumber;

internal sealed class GetOrderByNumberQueryHandler : IQueryHandler<GetOrderByNumberQuery, GetOrderByNumberResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetOrderByNumberQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetOrderByNumberResponse>> Handle(
        GetOrderByNumberQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
                            SELECT 
                             -- Order columns
                             o.id AS Id,
                             o.client_id AS ClientId,
                             o.order_number AS OrderNumber,
                             o.status AS Status,
                             o.total_amount AS TotalAmount,
                             o.paid_amount AS PaidAmount,
                             GREATEST(0, o.total_amount - o.paid_amount) AS OutstandingAmount,

                             -- Shipping address concatenated
                             CONCAT_WS(', ', 
                                 NULLIF(o.shipping_address_street, ''), 
                                 NULLIF(o.shipping_address_city, ''), 
                                 NULLIF(o.shipping_address_country, ''), 
                                 NULLIF(o.shipping_address_zipcode, '')
                             ) AS ShippingAddress,
                             
                             -- Other order columns
                             o.courier AS Courier,
                             o.created_at AS CreatedAt,
                             o.confirmed_at AS ConfirmedAt,
                             o.shipped_at AS ShippedAt,
                             o.delivered_at AS DeliveredAt,
                             o.cancelled_at AS CancelledAt,
                             o.estimated_delivery_date AS EstimatedDeliveryDate,
                             o.cancellation_reason AS CancellationReason,
                             o.return_reason AS ReturnReason,
                             o.refund_reason AS RefundReason,
                             o.info as Info,
                             o.tracking_number AS TrackingNumber,
                             o.currency AS Currency,
                             
                             -- Client data
                             u.id AS ClientId,
                             u.first_name AS ClientFirstName,
                             u.last_name AS ClientLastName,
                             u.email AS ClientEmail,
                             u.phone_number AS ClientPhoneNumber,
                             
                             -- Payment data (all payments)
                             p.id AS PaymentId,
                             p.amount AS PaymentTotalAmount,
                             p.payment_status AS PaymentStatus,
                             
                             -- Order Product data
                             op.product_id AS ProductId,
                             op.product_name AS ProductName,
                             op.quantity AS Quantity,
                             op.unit_price_amount AS UnitPriceAmount,
                             op.unit_price_currency AS UnitPriceCurrency,
                             op.total_price_amount AS TotalPriceAmount

                           FROM orders o
                           LEFT JOIN users u ON o.client_id = u.id
                           LEFT JOIN payments p ON o.id = p.order_id
                           LEFT JOIN order_products op ON o.id = op.order_id
                           WHERE o.order_number = @OrderNumber
                           ORDER BY p.created_at DESC
                           """;

        var orderDictionary = new Dictionary<Guid, GetOrderByNumberResponse>();

        await connection
            .QueryAsync<GetOrderByNumberResponse, ClientResponse, PaymentResponse, OrderProductResponse,
                GetOrderByNumberResponse>(
                sql,
                (order, client, payment, orderProduct) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out GetOrderByNumberResponse? existingOrder))
                    {
                        var orderProducts = new List<OrderProductResponse>();
                        if (orderProduct != null)
                        {
                            orderProducts.Add(orderProduct);
                        }

                        var payments = new List<PaymentResponse>();
                        if (payment != null)
                        {
                            payments.Add(payment);
                        }

                        GetOrderByNumberResponse newOrder = order with
                        {
                            Client = client,
                            Payments = payments,
                            OrderProducts = orderProducts
                        };
                        orderDictionary.Add(order.Id, newOrder);
                        return newOrder;
                    }

                    // Add payment if not already added
                    if (payment != null &&
                        !existingOrder.Payments!.Any(p => p.PaymentId == payment.PaymentId))
                    {
                        var updatedPayments = existingOrder.Payments!.ToList();
                        updatedPayments.Add(payment);
                        existingOrder = existingOrder with { Payments = updatedPayments };
                        orderDictionary[order.Id] = existingOrder;
                    }

                    // Add product if not already added
                    if (orderProduct != null &&
                        !existingOrder.OrderProducts.Any(p => p.ProductId == orderProduct.ProductId))
                    {
                        var updatedProducts = existingOrder.OrderProducts.ToList();
                        updatedProducts.Add(orderProduct);
                        existingOrder = existingOrder with { OrderProducts = updatedProducts };
                        orderDictionary[order.Id] = existingOrder;
                    }

                    return existingOrder;
                },
                new { request.OrderNumber },
                splitOn: "ClientId,PaymentId,ProductId"
            );

        GetOrderByNumberResponse? order = orderDictionary.Values.FirstOrDefault();

        if (order is null)
        {
            return Result.Failure<GetOrderByNumberResponse>(OrderErrors.NotFound);
        }

        return order;
    }
}
