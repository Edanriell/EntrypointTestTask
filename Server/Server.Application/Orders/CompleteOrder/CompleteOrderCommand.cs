using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.CompleteOrder;

public sealed record CompleteOrderCommand(Guid OrderId) : ICommand;
