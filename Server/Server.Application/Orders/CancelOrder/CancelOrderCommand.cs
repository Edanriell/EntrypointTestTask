using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.CancelOrder;

// Could be very complex
public sealed record CancelOrderCommand : ICommand
{
    public Guid OrderId { get; init; }
    public string CancellationReason { get; init; }
}
