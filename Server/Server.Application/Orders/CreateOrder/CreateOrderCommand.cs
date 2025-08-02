using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand : ICommand<Guid>
{
    public Guid ClientId { get; init; }
    public string OrderNumber { get; init; }
    public string Currency { get; init; }
    public ShippingAddress ShippingAddress { get; init; }
    public List<OrderItem> OrderItems { get; init; } = new();
    public string? Info { get; init; }
}

public sealed record ShippingAddress
{
    public string Country { get; init; }
    public string City { get; init; }
    public string ZipCode { get; init; }
    public string Street { get; init; }
}

public sealed record OrderItem
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
