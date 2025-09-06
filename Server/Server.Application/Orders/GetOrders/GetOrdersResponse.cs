namespace Server.Application.Orders.GetOrders;

public sealed record GetOrdersResponse
{
    public IReadOnlyList<OrdersResponse> Orders { get; init; } = new List<OrdersResponse>();
    public string? NextCursor { get; init; }
    public string? PreviousCursor { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
    public int TotalCount { get; init; }
    public int CurrentPageSize { get; init; }
    public int PageNumber { get; init; }
}

public sealed record OrdersResponse
{
    public Guid Id { get; init; }
    public Guid ClientId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
    public string? ShippingAddress { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ConfirmedAt { get; init; }
    public DateTime? ShippedAt { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public DateTime? CancelledAt { get; init; }
    public DateTime? EstimatedDeliveryDate { get; init; }
    public string? Currency { get; init; }
    public string? Courier { get; init; }
    public string? CancellationReason { get; init; }
    public string? ReturnReason { get; init; }
    public string? RefundReason { get; init; }
    public string? Info { get; init; }
    public string? TrackingNumber { get; init; }
    public ClientResponse? Client { get; init; }
    public IReadOnlyList<PaymentResponse>? Payments { get; init; } = new List<PaymentResponse>();
    public IReadOnlyList<OrderProductResponse> OrderProducts { get; init; } = new List<OrderProductResponse>();
}

public sealed record OrderProductResponse
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public string UnitPriceCurrency { get; init; } = string.Empty;
    public decimal UnitPriceAmount { get; init; }
    public decimal TotalPriceAmount { get; init; }
}

public sealed record ClientResponse
{
    public Guid ClientId { get; init; }
    public string ClientFirstName { get; init; } = string.Empty;
    public string ClientLastName { get; init; } = string.Empty;
    public string ClientEmail { get; init; } = string.Empty;
    public string? ClientPhoneNumber { get; init; }
}
 
public sealed record PaymentResponse
{
    public Guid PaymentId { get; init; }
    public decimal PaymentTotalAmount { get; init; }
    public string PaymentStatus { get; init; } = string.Empty;
}
