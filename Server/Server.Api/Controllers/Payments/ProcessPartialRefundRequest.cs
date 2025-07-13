namespace Server.Api.Controllers.Payments;

public record ProcessPartialRefundRequest(
    decimal RefundAmount,
    string Currency,
    string Reason);
