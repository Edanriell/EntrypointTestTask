using Server.Domain.Products;

namespace Server.Application.Products.GetProductById;

public sealed class GetProductByIdResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; }
    public int TotalStock { get; init; }
    public int Reserved { get; init; }
    public int Available { get; init; }
    public bool IsOutOfStock { get; init; }
    public bool HasReservations { get; init; }
    public bool IsInStock { get; init; }
    public ProductStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public DateTime? LastRestockedAt { get; init; }
}
 
