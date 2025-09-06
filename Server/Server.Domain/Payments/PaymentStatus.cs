namespace Server.Domain.Payments;

public enum PaymentStatus
{
    Pending,
    Processing,
    Paid,
    Failed,
    Cancelled,
    Refunded,
    Expired
}
 
