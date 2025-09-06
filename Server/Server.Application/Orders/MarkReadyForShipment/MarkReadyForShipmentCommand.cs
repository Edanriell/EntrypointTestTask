using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.MarkReadyForShipment;

public sealed record MarkReadyForShipmentCommand(Guid OrderId) : ICommand;
 
  
