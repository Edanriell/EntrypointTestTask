using Server.Application.Abstractions.Messaging;
using Server.Domain.Payments;

namespace Server.Application.Payments.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod,  // This should be string, not PaymentMethod enum
    string? PaymentReference = null) : ICommand<Guid>;
