using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.ProcessPayment;

public sealed record ProcessPaymentCommand(Guid PaymentId) : ICommand;
