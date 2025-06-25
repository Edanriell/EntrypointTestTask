namespace Server.Api.Controllers.Orders;

public record CreateOrderRequest(
    Guid ClientId,
    ShippingAddressRequest ShippingAddress,
    IReadOnlyList<OrderItemRequest> OrderItems
);

public record ShippingAddressRequest(
    string Country,
    string City,
    string ZipCode,
    string Street
);

public record OrderItemRequest(
    Guid ProductId,
    int Quantity
);
