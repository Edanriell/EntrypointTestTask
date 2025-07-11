namespace Server.Domain.Payments;

public enum PaymentStatus
{
    Pending,
    Processing,
    Paid,
    PartiallyPaid,
    Failed,
    Cancelled,
    Refunded,
    PartiallyRefunded,
    Disputed,
    Expired
}
