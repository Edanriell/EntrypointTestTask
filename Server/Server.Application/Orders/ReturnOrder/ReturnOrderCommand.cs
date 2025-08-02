using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.ReturnOrder;

public sealed record ReturnOrderCommand : ICommand
{
    public Guid OrderId { get; init; }

    public string ReturnReason { get; init; }
}
