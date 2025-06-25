using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.ShipOrder;

// Could be much more complex
public sealed record ShipOrderCommand : ICommand
{
    public Guid OrderId { get; init; }
    public string TrackingNumber { get; init; } = string.Empty;
}
