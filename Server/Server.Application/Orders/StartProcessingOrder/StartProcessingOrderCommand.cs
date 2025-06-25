using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.StartProcessingOrder;

public sealed record StartProcessingOrderCommand : ICommand
{
    public Guid OrderId { get; init; }
}
