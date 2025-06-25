using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.MarkOrderAsDelivered;

public sealed record MarkOrderAsDeliveredCommand : ICommand
{
    public Guid OrderId { get; init; }
    // Command could be much more complex
    // public string? DeliveryNotes { get; init; }
    // public string? DeliveredBy { get; init; }
    // public string? ReceivedBy { get; init; }
    // public DateTime? DeliveryDate { get; init; }
}
