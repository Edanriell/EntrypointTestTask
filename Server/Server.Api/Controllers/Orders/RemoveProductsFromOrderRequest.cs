namespace Server.Api.Controllers.Orders;

public sealed record RemoveProductsFromOrderRequest
{
    public List<ProductRemovalDto> ProductRemovals { get; init; } = new();
}

public sealed record ProductRemovalDto
{
    public Guid ProductId { get; init; }
    public int? Quantity { get; init; } // null = remove all
}
 
