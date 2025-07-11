using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.MarkPaymentAsExpired;

public sealed record MarkPaymentAsExpiredCommand(Guid PaymentId) : ICommand;
