using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.UpdateOrderShippingAddress;

public sealed record UpdateOrderShippingAddressCommand : ICommand
{
    public Guid OrderId { get; init; }
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
}
