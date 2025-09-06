using Server.Application.Abstractions.Messaging;
using Server.Domain.Orders;

namespace Server.Application.Orders.ShipOrder;

// Could be much more complex
public sealed record ShipOrderCommand(
    Guid OrderId,
    string TrackingNumber,
    Courier Courier,
    DateTime EstimatedDeliveryDate
) : ICommand;
 
