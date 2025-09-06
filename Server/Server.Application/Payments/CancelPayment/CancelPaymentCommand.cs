using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.CancelPayment;

public sealed record CancelPaymentCommand(Guid PaymentId) : ICommand;
 
