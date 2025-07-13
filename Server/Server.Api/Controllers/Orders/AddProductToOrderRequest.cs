namespace Server.Api.Controllers.Orders;

public record AddProductToOrderRequest(
    IReadOnlyList<ProductItemRequest> Products
);
  
public record ProductItemRequest(
    Guid ProductId,
    int Quantity
);
