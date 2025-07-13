namespace Server.Application.Payments.GetPaymentsByOrderId;

public sealed record GetPaymentsByOrderIdResponse(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentStatus,
    string PaymentMethod,
    string? PaymentReference,
    DateTime CreatedAt,
    DateTime? PaymentCompletedAt);
