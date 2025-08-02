namespace Server.Api.Controllers.Payments;

public sealed record RefundOrderPaymentsRequest(
    string RefundReason
);
