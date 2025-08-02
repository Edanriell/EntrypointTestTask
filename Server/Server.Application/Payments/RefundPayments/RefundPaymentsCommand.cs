using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.ProcessOrderRefundCommand;

public sealed record RefundPaymentsCommand(Guid OrderId, string RefundReason) : ICommand;
