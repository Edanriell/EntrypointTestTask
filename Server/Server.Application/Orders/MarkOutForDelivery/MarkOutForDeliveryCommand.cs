using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.MarkOutForDelivery;

public sealed record MarkOutForDeliveryCommand(
    Guid OrderId,
    DateTime? EstimatedDeliveryDate = null
) : ICommand;
 
