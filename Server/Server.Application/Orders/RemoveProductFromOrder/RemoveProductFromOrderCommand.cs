using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.RemoveProductFromOrder;

public sealed record RemoveProductFromOrderCommand : ICommand
{
    public Guid OrderId { get; init; }
    public List<ProductRemovalRequest> ProductRemovals { get; init; } = new();
}

public sealed record ProductRemovalRequest
{
    public Guid ProductId { get; init; }
    public int? Quantity { get; init; } // null = remove all
}
 
