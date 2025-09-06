using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.AddProductToOrder;

public sealed record AddProductToOrderCommand(
    Guid OrderId,
    IReadOnlyList<ProductItem> Products) : ICommand;

public sealed record ProductItem(
    Guid ProductId,
    int Quantity);
 
