using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.MarkPaymentAsDisputed;

public sealed record MarkPaymentAsDisputedCommand(Guid PaymentId, string DisputeReason) : ICommand;
