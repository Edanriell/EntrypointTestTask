using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.AddPayment;

public sealed record AddPaymentCommand(
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod) : ICommand<Guid>;
 
