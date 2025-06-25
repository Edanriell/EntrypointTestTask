namespace Server.Domain.Payments;

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Paid = 3,
    PartiallyPaid = 4,
    Failed = 5,
    Cancelled = 6,
    Refunded = 7,
    PartiallyRefunded = 8,
    Disputed = 9
}
