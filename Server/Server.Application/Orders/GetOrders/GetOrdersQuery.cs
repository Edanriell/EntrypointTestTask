using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.GetOrders;

public sealed record GetOrdersQuery : IQuery<GetOrdersResponse>
{
    public int PageSize { get; init; } = 10;
    public string? Cursor { get; init; }
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }

    // Order filtering properties
    public string? OrderNumberFilter { get; init; }
    public string? StatusFilter { get; init; }
    public decimal? MinTotalAmount { get; init; }
    public decimal? MaxTotalAmount { get; init; }
    public string? TrackingNumberFilter { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public DateTime? CreatedBefore { get; init; }
    public DateTime? ConfirmedAfter { get; init; }
    public DateTime? ConfirmedBefore { get; init; }
    public DateTime? ShippedAfter { get; init; }
    public DateTime? ShippedBefore { get; init; }
    public DateTime? DeliveredAfter { get; init; }
    public DateTime? DeliveredBefore { get; init; }
    public decimal? MinOutstandingAmount { get; set; }
    public decimal? MaxOutstandingAmount { get; set; }
    public DateTime? EstimatedDeliveryAfter { get; set; }
    public DateTime? EstimatedDeliveryBefore { get; set; }
    public bool? HasPayment { get; set; }
    public bool? IsFullyPaid { get; set; }
    public bool? HasOutstandingBalance { get; set; }

    // Product filtering properties
    public string? ProductNameFilter { get; set; }
    public Guid? ProductIdFilter { get; set; }

    // Client filtering properties
    public string? ClientEmailFilter { get; init; }
    public string? ClientNameFilter { get; init; }

    // Payment filtering properties
    public string? PaymentStatusFilter { get; init; }
}
