// Needs refactoring! 

using System.Data;
using System.Globalization;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Application.Abstractions.Pagination;
using Server.Domain.Abstractions;

namespace Server.Application.Orders.GetOrders;

internal sealed class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, GetOrdersResponse>
{
    private readonly ICursorPaginationService _cursorPaginationService;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetOrdersQueryHandler(
        ISqlConnectionFactory sqlConnectionFactory,
        ICursorPaginationService cursorPaginationService)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _cursorPaginationService = cursorPaginationService;
    }

    public async Task<Result<GetOrdersResponse>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        var sqlBuilder = new StringBuilder();
        var parameters = new DynamicParameters();

        // Calculate pagination parameters first
        CursorInfo cursorInfo = _cursorPaginationService.DecodeCursor(request.Cursor ?? string.Empty);
        int offset = (cursorInfo.PageNumber - 1) * request.PageSize;

        // Build the main query with CTE to paginate orders first
        sqlBuilder.Append("""
                          WITH paginated_orders AS (
                              SELECT DISTINCT
                                o.id AS Id,
                                o.client_id AS ClientId,
                                o.order_number AS OrderNumber,
                                o.status AS Status,
                                o.total_amount AS TotalAmount,
                                o.paid_amount AS PaidAmount,
                                GREATEST(0, o.total_amount - o.paid_amount) AS OutstandingAmount,
                                CONCAT_WS(', ', 
                                    NULLIF(o.shipping_address_street, ''), 
                                    NULLIF(o.shipping_address_city, ''), 
                                    NULLIF(o.shipping_address_country, ''), 
                                    NULLIF(o.shipping_address_zipcode, '')
                                ) AS ShippingAddress,
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
                                o.info AS Info,
                                o.tracking_number AS TrackingNumber,
                                o.currency AS Currency
                              FROM orders o
                              LEFT JOIN users u ON o.client_id = u.id
                              LEFT JOIN payments p ON o.id = p.order_id
                              WHERE 1=1
                          """);

        // Add filtering
        AddFiltersToBaseQuery(sqlBuilder, parameters, request);

        // Add sorting
        AddSortingForCTE(sqlBuilder, request);

        // Add pagination, limits ORDERS, not joined rows
        parameters.Add("PageSize", request.PageSize + 1); // +1 to check for next page
        parameters.Add("Offset", offset);
        sqlBuilder.Append(" LIMIT @PageSize OFFSET @Offset");

        sqlBuilder.Append("""
                          )
                          SELECT 
                            -- Order columns from CTE
                            po.Id,
                            po.ClientId,
                            po.OrderNumber,
                            po.Status,
                            po.TotalAmount,
                            po.PaidAmount,
                            po.OutstandingAmount,
                            po.ShippingAddress,
                            po.Courier,
                            po.CreatedAt,
                            po.ConfirmedAt,
                            po.ShippedAt,
                            po.DeliveredAt,
                            po.CancelledAt,
                            po.EstimatedDeliveryDate,
                            po.CancellationReason,
                            po.ReturnReason,
                            po.RefundReason,
                            po.Info,
                            po.TrackingNumber,
                            po.currency AS Currency,
                            
                            -- Client data
                            u.id AS ClientId,
                            u.first_name AS ClientFirstName,
                            u.last_name AS ClientLastName,
                            u.email AS ClientEmail,
                            u.phone_number AS ClientPhoneNumber,
                            
                            -- Payment data (all payments for each order)
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

                          FROM paginated_orders po
                          LEFT JOIN users u ON po.ClientId = u.id
                          LEFT JOIN payments p ON po.Id = p.order_id
                          LEFT JOIN order_products op ON po.Id = op.order_id
                          """);

        // Add the same sorting to the final result to maintain order
        AddSortingForFinalQuery(sqlBuilder, request);

        var orderDictionary = new Dictionary<Guid, OrdersResponse>();

        await connection
            .QueryAsync<OrdersResponse, ClientResponse, PaymentResponse, OrderProductResponse, OrdersResponse>(
                sqlBuilder.ToString(),
                (order, client, payment, orderProduct) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out OrdersResponse? existingOrder))
                    {
                        // Create payments list
                        IReadOnlyList<PaymentResponse> payments = payment != null && payment.PaymentId != Guid.Empty
                            ? new List<PaymentResponse> { payment }
                            : new List<PaymentResponse>();

                        // Create order products list
                        IReadOnlyList<OrderProductResponse> orderProducts =
                            orderProduct != null && orderProduct.ProductId != Guid.Empty
                                ? new List<OrderProductResponse> { orderProduct }
                                : new List<OrderProductResponse>();

                        OrdersResponse newOrder = order with
                        {
                            Client = client,
                            Payments = payments,
                            OrderProducts = orderProducts
                        };
                        orderDictionary.Add(order.Id, newOrder);
                        return newOrder;
                    }

                    // Add payment to existing order if not already added
                    if (payment != null &&
                        payment.PaymentId != Guid.Empty &&
                        existingOrder.Payments != null &&
                        !existingOrder.Payments.Any(p => p.PaymentId == payment.PaymentId))
                    {
                        var updatedPayments = existingOrder.Payments.ToList();
                        updatedPayments.Add(payment);

                        OrdersResponse updatedOrder = existingOrder with { Payments = updatedPayments };
                        orderDictionary[order.Id] = updatedOrder;
                    }

                    // Add product to existing order if not already added
                    if (orderProduct != null &&
                        orderProduct.ProductId != Guid.Empty &&
                        !existingOrder.OrderProducts.Any(p => p.ProductId == orderProduct.ProductId))
                    {
                        var updatedProducts = existingOrder.OrderProducts.ToList();
                        updatedProducts.Add(orderProduct);

                        OrdersResponse updatedOrder = orderDictionary[order.Id] with
                        {
                            OrderProducts = updatedProducts
                        };
                        orderDictionary[order.Id] = updatedOrder;
                        return updatedOrder;
                    }

                    return existingOrder;
                },
                parameters,
                splitOn: "ClientId,PaymentId,ProductId"
            );


        var orders = orderDictionary.Values.ToList();

        // Determine pagination state
        PaginationInfo<OrdersResponse> paginationInfo = _cursorPaginationService.DeterminePaginationState(
            orders,
            request.PageSize,
            request.SortBy ?? "CreatedAt",
            cursorInfo.PageNumber,
            order => GetOrderSortValue(order, request.SortBy ?? "CreatedAt"));

        // Get total count
        int totalCount = await GetTotalCount(connection, request);

        return new GetOrdersResponse
        {
            Orders = paginationInfo.PageItems,
            NextCursor = paginationInfo.NextCursor,
            PreviousCursor = paginationInfo.PreviousCursor,
            HasNextPage = paginationInfo.HasNextPage,
            HasPreviousPage = paginationInfo.HasPreviousPage,
            TotalCount = totalCount,
            CurrentPageSize = paginationInfo.PageItems.Count,
            PageNumber = cursorInfo.PageNumber
        };
    }

    private void AddFiltersToBaseQuery(StringBuilder sqlBuilder, DynamicParameters parameters, GetOrdersQuery request)
    {
        if (!string.IsNullOrEmpty(request.OrderNumberFilter))
        {
            sqlBuilder.Append(" AND o.order_number ILIKE @OrderNumberFilter");
            parameters.Add("OrderNumberFilter", $"%{request.OrderNumberFilter}%");
        }

        if (!string.IsNullOrEmpty(request.StatusFilter))
        {
            sqlBuilder.Append(" AND o.status ILIKE @StatusFilter");
            parameters.Add("StatusFilter", $"%{request.StatusFilter}%");
        }

        if (request.MinTotalAmount.HasValue)
        {
            sqlBuilder.Append(" AND o.total_amount >= @MinTotalAmount");
            parameters.Add("MinTotalAmount", request.MinTotalAmount.Value);
        }

        if (request.MaxTotalAmount.HasValue)
        {
            sqlBuilder.Append(" AND o.total_amount <= @MaxTotalAmount");
            parameters.Add("MaxTotalAmount", request.MaxTotalAmount.Value);
        }

        if (!string.IsNullOrEmpty(request.ClientEmailFilter))
        {
            sqlBuilder.Append(" AND u.email ILIKE @ClientEmailFilter");
            parameters.Add("ClientEmailFilter", $"%{request.ClientEmailFilter}%");
        }

        if (!string.IsNullOrEmpty(request.ClientNameFilter))
        {
            sqlBuilder.Append(" AND (u.first_name ILIKE @ClientNameFilter OR u.last_name ILIKE @ClientNameFilter)");
            parameters.Add("ClientNameFilter", $"%{request.ClientNameFilter}%");
        }

        if (!string.IsNullOrEmpty(request.TrackingNumberFilter))
        {
            sqlBuilder.Append(" AND o.tracking_number ILIKE @TrackingNumberFilter");
            parameters.Add("TrackingNumberFilter", $"%{request.TrackingNumberFilter}%");
        }

        if (request.CreatedAfter.HasValue)
        {
            sqlBuilder.Append(" AND o.created_at >= @CreatedAfter");
            parameters.Add("CreatedAfter", request.CreatedAfter.Value);
        }

        if (request.CreatedBefore.HasValue)
        {
            sqlBuilder.Append(" AND o.created_at <= @CreatedBefore");
            parameters.Add("CreatedBefore", request.CreatedBefore.Value);
        }

        if (request.ConfirmedAfter.HasValue)
        {
            sqlBuilder.Append(" AND o.confirmed_at >= @ConfirmedAfter");
            parameters.Add("ConfirmedAfter", request.ConfirmedAfter.Value);
        }

        if (request.ConfirmedBefore.HasValue)
        {
            sqlBuilder.Append(" AND o.confirmed_at <= @ConfirmedBefore");
            parameters.Add("ConfirmedBefore", request.ConfirmedBefore.Value);
        }

        if (request.ShippedAfter.HasValue)
        {
            sqlBuilder.Append(" AND o.shipped_at >= @ShippedAfter");
            parameters.Add("ShippedAfter", request.ShippedAfter.Value);
        }

        if (request.ShippedBefore.HasValue)
        {
            sqlBuilder.Append(" AND o.shipped_at <= @ShippedBefore");
            parameters.Add("ShippedBefore", request.ShippedBefore.Value);
        }

        if (request.DeliveredAfter.HasValue)
        {
            sqlBuilder.Append(" AND o.delivered_at >= @DeliveredAfter");
            parameters.Add("DeliveredAfter", request.DeliveredAfter.Value);
        }

        if (request.DeliveredBefore.HasValue)
        {
            sqlBuilder.Append(" AND o.delivered_at <= @DeliveredBefore");
            parameters.Add("DeliveredBefore", request.DeliveredBefore.Value);
        }

        if (!string.IsNullOrEmpty(request.PaymentStatusFilter))
        {
            sqlBuilder.Append(" AND p.payment_status ILIKE @PaymentStatusFilter");
            parameters.Add("PaymentStatusFilter", $"%{request.PaymentStatusFilter}%");
        }

        // New filters
        if (request.MinOutstandingAmount.HasValue)
        {
            sqlBuilder.Append(" AND GREATEST(0, o.total_amount - o.paid_amount) >= @MinOutstandingAmount");
            parameters.Add("MinOutstandingAmount", request.MinOutstandingAmount.Value);
        }

        if (request.MaxOutstandingAmount.HasValue)
        {
            sqlBuilder.Append(" AND GREATEST(0, o.total_amount - o.paid_amount) <= @MaxOutstandingAmount");
            parameters.Add("MaxOutstandingAmount", request.MaxOutstandingAmount.Value);
        }

        if (request.EstimatedDeliveryAfter.HasValue)
        {
            sqlBuilder.Append(" AND o.estimated_delivery_date >= @EstimatedDeliveryAfter");
            parameters.Add("EstimatedDeliveryAfter", request.EstimatedDeliveryAfter.Value);
        }

        if (request.EstimatedDeliveryBefore.HasValue)
        {
            sqlBuilder.Append(" AND o.estimated_delivery_date <= @EstimatedDeliveryBefore");
            parameters.Add("EstimatedDeliveryBefore", request.EstimatedDeliveryBefore.Value);
        }

        if (request.HasPayment.HasValue)
        {
            if (request.HasPayment.Value)
            {
                sqlBuilder.Append(" AND p.id IS NOT NULL");
            }
            else
            {
                sqlBuilder.Append(" AND p.id IS NULL");
            }
        }

        if (request.IsFullyPaid.HasValue)
        {
            if (request.IsFullyPaid.Value)
            {
                sqlBuilder.Append(" AND o.paid_amount >= o.total_amount");
            }
            else
            {
                sqlBuilder.Append(" AND o.paid_amount < o.total_amount");
            }
        }

        if (request.HasOutstandingBalance.HasValue)
        {
            if (request.HasOutstandingBalance.Value)
            {
                sqlBuilder.Append(" AND o.paid_amount < o.total_amount");
            }
            else
            {
                sqlBuilder.Append(" AND o.paid_amount >= o.total_amount");
            }
        }

        if (!string.IsNullOrEmpty(request.ProductNameFilter))
        {
            sqlBuilder.Append(
                " AND EXISTS (SELECT 1 FROM order_products op_filter WHERE op_filter.order_id = o.id AND op_filter.product_name ILIKE @ProductNameFilter)");
            parameters.Add("ProductNameFilter", $"%{request.ProductNameFilter}%");
        }

        if (request.ProductIdFilter.HasValue)
        {
            sqlBuilder.Append(
                " AND EXISTS (SELECT 1 FROM order_products op_filter WHERE op_filter.order_id = o.id AND op_filter.product_id = @ProductIdFilter)");
            parameters.Add("ProductIdFilter", request.ProductIdFilter.Value);
        }
    }

    private void AddFilters(StringBuilder sqlBuilder, DynamicParameters parameters, GetOrdersQuery request)
    {
        AddFiltersToBaseQuery(sqlBuilder, parameters, request);
    }

    private void AddSortingForCTE(StringBuilder sqlBuilder, GetOrdersQuery request)
    {
        string sortColumn = GetSortColumnForCTE(request.SortBy);
        string sortDirection = request.SortDirection?.ToUpper() == "ASC" ? "ASC" : "DESC";

        sqlBuilder.Append(CultureInfo.InvariantCulture, $" ORDER BY {sortColumn} {sortDirection}");
    }

    private void AddSortingForFinalQuery(StringBuilder sqlBuilder, GetOrdersQuery request)
    {
        string sortColumn = GetSortColumnForFinalQuery(request.SortBy);
        string sortDirection = request.SortDirection?.ToUpper() == "ASC" ? "ASC" : "DESC";

        sqlBuilder.Append(CultureInfo.InvariantCulture, $" ORDER BY {sortColumn} {sortDirection}");
    }

    private string GetSortColumnForCTE(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "ordernumber" => "o.order_number",
            "status" => "o.status",
            "totalamount" => "o.total_amount",
            "paidamount" => "o.paid_amount",
            "outstandingamount" => "GREATEST(0, o.total_amount - o.paid_amount)",
            "createdat" => "o.created_at",
            "confirmedat" => "o.confirmed_at",
            "shippedat" => "o.shipped_at",
            "deliveredat" => "o.delivered_at",
            "cancelledat" => "o.cancelled_at",
            "estimateddeliverydate" => "o.estimated_delivery_date",
            "clientfirstname" => "u.first_name",
            "clientlastname" => "u.last_name",
            "clientemail" => "u.email",
            "paymentstatus" => "p.payment_status",
            "paymentamount" => "p.amount",
            "courier" => "o.courier",
            "trackingnumber" => "o.tracking_number",
            _ => "o.created_at"
        };
    }

    private string GetSortColumnForFinalQuery(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "ordernumber" => "po.OrderNumber",
            "status" => "po.Status",
            "totalamount" => "po.TotalAmount",
            "paidamount" => "po.PaidAmount",
            "outstandingamount" => "po.OutstandingAmount",
            "createdat" => "po.CreatedAt",
            "confirmedat" => "po.ConfirmedAt",
            "shippedat" => "po.ShippedAt",
            "deliveredat" => "po.DeliveredAt",
            "cancelledat" => "po.CancelledAt",
            "estimateddeliverydate" => "po.EstimatedDeliveryDate",
            "clientfirstname" => "u.first_name",
            "clientlastname" => "u.last_name",
            "clientemail" => "u.email",
            "paymentstatus" => "p.payment_status",
            "paymentamount" => "p.amount",
            "courier" => "po.Courier",
            "trackingnumber" => "po.TrackingNumber",
            _ => "po.CreatedAt"
        };
    }

    private async Task<int> GetTotalCount(IDbConnection connection, GetOrdersQuery request)
    {
        var sqlBuilder = new StringBuilder();
        var parameters = new DynamicParameters();

        sqlBuilder.Append("""
                          SELECT COUNT(DISTINCT o.id)
                          FROM orders o
                          LEFT JOIN users u ON o.client_id = u.id
                          LEFT JOIN payments p ON o.id = p.order_id
                          WHERE 1=1
                          """);

        // Add the same filters as the main query
        AddFilters(sqlBuilder, parameters, request);

        return await connection.QuerySingleAsync<int>(sqlBuilder.ToString(), parameters);
    }

    private object GetOrderSortValue(OrdersResponse order, string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "ordernumber" => order.OrderNumber,
            "status" => order.Status,
            "totalamount" => order.TotalAmount,
            "paidamount" => order.PaidAmount,
            "outstandingamount" => order.OutstandingAmount,
            "createdat" => order.CreatedAt,
            "confirmedat" => order.ConfirmedAt,
            "shippedat" => order.ShippedAt,
            "deliveredat" => order.DeliveredAt,
            "cancelledat" => order.CancelledAt,
            "estimateddeliverydate" => order.EstimatedDeliveryDate,
            "clientfirstname" => order.Client?.ClientFirstName ?? string.Empty,
            "clientlastname" => order.Client?.ClientLastName ?? string.Empty,
            "clientemail" => order.Client?.ClientEmail ?? string.Empty,
            "paymentstatus" => order.Payments != null && order.Payments.Count > 0
                ? order.Payments[0].PaymentStatus
                : string.Empty,
            "paymentamount" => order.Payments != null && order.Payments.Count > 0
                ? order.Payments[0].PaymentTotalAmount
                : 0,
            "courier" => order.Courier ?? string.Empty,
            "trackingnumber" => order.TrackingNumber ?? string.Empty,
            _ => order.CreatedAt
        };
    }
}
