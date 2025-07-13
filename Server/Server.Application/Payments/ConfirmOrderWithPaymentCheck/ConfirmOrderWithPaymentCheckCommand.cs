using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.ConfirmOrderWithPaymentCheck;

public sealed record ConfirmOrderWithPaymentCheckCommand(Guid OrderId) : ICommand;
