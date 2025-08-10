using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.RefundPayments;

public sealed record RefundPaymentsCommand(Guid OrderId, string RefundReason) : ICommand;
