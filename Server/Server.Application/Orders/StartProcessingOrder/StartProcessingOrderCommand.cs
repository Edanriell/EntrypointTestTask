using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.StartProcessingOrder;

public sealed record StartProcessingOrderCommand(Guid OrderId) : ICommand;
 
