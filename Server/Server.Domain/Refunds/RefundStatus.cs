namespace Server.Domain.Refunds;

public enum RefundStatus
{
    Pending, // Refund created but not processed
    Processing, // Refund being processed by payment gateway
    Processed, // Refund successfully processed
    Failed // Refund failed
}
