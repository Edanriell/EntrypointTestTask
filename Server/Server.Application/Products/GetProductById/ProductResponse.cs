using Server.Domain.Products;

namespace Server.Application.Products.GetProductById;

public sealed class ProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public int Stock { get; init; }
    public ProductStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public DateTime? LastRestockedAt { get; init; }
}
