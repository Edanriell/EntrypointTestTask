using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.UpdateOrderProductQuantity;

public sealed record UpdateOrderProductQuantityCommand : ICommand
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public int NewQuantity { get; init; }
}
