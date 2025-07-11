namespace Server.Application.Products.GetProducts;

public sealed record GetProductsResponse
{
    public IReadOnlyList<ProductsResponse> Products { get; init; } = new List<ProductsResponse>();
    public string? NextCursor { get; init; }
    public string? PreviousCursor { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
    public int TotalCount { get; init; }
    public int CurrentPageSize { get; init; }
    public int PageNumber { get; init; }
}

public sealed record ProductsResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Reserved { get; init; }
    public int Stock { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdatedAt { get; init; }

    public DateTime? LastRestockedAt { get; init; }

    // Computed property
    public int AvailableQuantity => Stock - Reserved;
}
