namespace Server.Api.Controllers.Payments;

public record RefundPaymentWithOrderUpdateRequest(
    decimal RefundAmount,
    string RefundReason
);
