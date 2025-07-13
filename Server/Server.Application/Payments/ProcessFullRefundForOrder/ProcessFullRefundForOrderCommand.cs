using Server.Application.Abstractions.Messaging;

namespace Server.Application.Payments.ProcessFullRefundForOrder;

public sealed record ProcessFullRefundForOrderCommand(Guid OrderId, string Reason) : ICommand;
