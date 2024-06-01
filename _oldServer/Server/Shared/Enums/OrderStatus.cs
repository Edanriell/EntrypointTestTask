namespace Server.Shared.Enums
{
    public enum OrderStatus
    {
        Created,
        PendingForPayment,
        Paid,
        InTransit,
        Delivered,
        Cancelled
    }
}
