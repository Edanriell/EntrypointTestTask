using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.ReturnOrder;

public sealed record ReturnOrderCommand : ICommand
{
    public Guid OrderId { get; init; }

    public string ReturnReason { get; init; } = string.Empty;
    // Potentially complex
    // public List<ReturnItem> ReturnItems { get; init; } = new();
    // public string? CustomerComments { get; init; }
    // public bool IsRefundRequested { get; init; } = true;
    // public string? RequestedBy { get; init; }
}
