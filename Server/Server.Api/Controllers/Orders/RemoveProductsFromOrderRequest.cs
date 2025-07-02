namespace Server.Api.Controllers.Orders;

public record RemoveProductsFromOrderRequest(
    IReadOnlyList<Guid> ProductIds
);
 
