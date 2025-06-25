using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.RefundPaymentWithOrderUpdate;

public sealed record RefundPaymentWithOrderUpdateCommand : ICommand
{
    public Guid OrderId { get; init; }
    public decimal RefundAmount { get; init; }
    public string RefundReason { get; init; } = string.Empty;
}
