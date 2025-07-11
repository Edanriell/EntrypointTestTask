using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.FailPayment;

public sealed record FailPaymentCommand(Guid PaymentId, string Reason) : ICommand;
