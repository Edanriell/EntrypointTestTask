using Server.Application.Abstractions.Messaging;
using Server.Domain.Payments;

namespace Server.Application.Payments.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid OrderId,
    decimal Amount,
    string Currency,
    PaymentMethod PaymentMethod,
    string? PaymentReference = null
) : ICommand<Guid>;
