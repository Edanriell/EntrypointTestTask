using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.DeleteOrder;

public sealed record DeleteOrderCommand : ICommand
{
    public Guid OrderId { get; init; }
}
  
