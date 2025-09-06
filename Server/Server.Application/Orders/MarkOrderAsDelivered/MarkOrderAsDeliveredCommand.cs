using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.MarkOrderAsDelivered;

public sealed record MarkOrderAsDeliveredCommand(Guid OrderId) : ICommand;
  
