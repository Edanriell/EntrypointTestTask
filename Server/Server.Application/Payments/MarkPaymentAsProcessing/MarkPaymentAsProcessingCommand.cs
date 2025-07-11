using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.MarkPaymentAsProcessing;

public sealed record MarkPaymentAsProcessingCommand(Guid PaymentId) : ICommand;
