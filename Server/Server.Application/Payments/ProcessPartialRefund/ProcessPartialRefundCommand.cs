using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.ProcessPartialRefund;

public sealed record ProcessPartialRefundCommand(
    Guid PaymentId,
    decimal RefundAmount,
    string Currency,
    string Reason) : ICommand<Guid>;
