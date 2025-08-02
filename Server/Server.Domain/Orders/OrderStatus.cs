namespace Server.Domain.Orders;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Processing,
    ReadyForShipment,
    Shipped,
    OutForDelivery,
    Delivered,
    Completed,
    Cancelled,
    Returned,
    Failed,
    UnderReview
}
