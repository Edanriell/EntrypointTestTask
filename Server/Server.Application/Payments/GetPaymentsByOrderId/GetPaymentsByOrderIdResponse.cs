namespace Server.Application.Payments.GetPaymentsByOrderId;

public sealed record GetPaymentsByOrderIdResponse
{
    public Guid OrderId { get; init; }
    public IReadOnlyList<PaymentResponse> Payments { get; init; } = new List<PaymentResponse>();
}

public sealed record PaymentResponse
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? PaymentCompletedAt { get; init; }
    public DateTime? PaymentFailedAt { get; init; }
    public DateTime? PaymentExpiredAt { get; init; }
    public string? PaymentFailureReason { get; init; }
    public RefundResponse? Refund { get; init; }
}

public sealed record RefundResponse
{
    public Guid RefundId { get; init; }
    public decimal RefundAmount { get; init; }
    public string RefundCurrency { get; init; } = string.Empty;
    public string RefundReason { get; init; } = string.Empty;
    public DateTime RefundCreatedAt { get; init; }
    public DateTime? RefundProcessedAt { get; init; }
}
