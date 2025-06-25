using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.ProcessPaymentWithAutomaticOrderConfirmation;

public sealed record ProcessPaymentWithAutomaticOrderConfirmationCommand : ICommand
{
    public Guid OrderId { get; init; }
    public decimal PaymentAmount { get; init; }
}
