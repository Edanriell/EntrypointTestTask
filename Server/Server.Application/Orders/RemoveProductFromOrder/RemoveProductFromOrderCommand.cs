using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.RemoveProductFromOrder;

public sealed record RemoveProductFromOrderCommand : ICommand
{
    public Guid OrderId { get; init; }
    public List<Guid> ProductIds { get; init; } = new();
}
