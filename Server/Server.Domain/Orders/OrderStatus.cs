namespace Server.Domain.Orders;

public enum OrderStatus
{
    // Order created, awaiting payment
    Pending,

    // Payment successful, order confirmed
    Confirmed,

    // Order being prepared/packed
    Processing,

    // Packed and ready to ship
    ReadyForShipment,

    // Order shipped, in transit
    Shipped,

    // Out for delivery
    OutForDelivery,

    // Successfully delivered
    Delivered,

    // Order fully completed (delivered + no returns)
    Completed,

    // Order cancelled
    Cancelled,

    // Order returned
    Returned
}
