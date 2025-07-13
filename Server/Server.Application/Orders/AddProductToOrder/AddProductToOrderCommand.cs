using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.AddProductToOrder;

public sealed record AddProductToOrderCommand : ICommand
{
    public Guid OrderId { get; init; }
    public List<ProductItem> Products { get; init; } = new();
}
 
public sealed record ProductItem
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
