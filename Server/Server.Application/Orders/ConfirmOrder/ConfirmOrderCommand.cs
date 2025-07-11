using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.ConfirmOrder;

public sealed record ConfirmOrderCommand(Guid OrderId) : ICommand;
