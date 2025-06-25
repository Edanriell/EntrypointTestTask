namespace Server.Application.Payments.GetPaymentById;

public sealed record PaymentResponse
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public decimal TotalAmount { get; init; }
    public string PaymentStatus { get; init; } = string.Empty;
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
    public DateTime? PaymentCompletedAt { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? ModifiedOnUtc { get; init; }
}
