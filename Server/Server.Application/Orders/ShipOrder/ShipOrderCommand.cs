using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.ShipOrder;

// Could be much more complex
public sealed record ShipOrderCommand(Guid OrderId, string TrackingNumber) : ICommand;
