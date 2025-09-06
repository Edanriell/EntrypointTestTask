namespace Server.Application.Products.GetProducts;

public sealed record GetProductsResponse
{
    public IReadOnlyList<GetAllProductsResponse> Products { get; init; } = new List<GetAllProductsResponse>();
    public string? NextCursor { get; init; }
    public string? PreviousCursor { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
    public int TotalCount { get; init; }
    public int CurrentPageSize { get; init; }
    public int PageNumber { get; init; }
}

public sealed record GetAllProductsResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; }
    public int TotalStock { get; init; }
    public int Reserved { get; init; }
    public int Available { get; init; }
    public bool IsOutOfStock { get; init; }
    public bool HasReservations { get; init; }
    public bool IsInStock { get; init; }
    public bool HasLowStock { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdatedAt { get; init; }

    public DateTime? LastRestockedAt { get; init; }
}
