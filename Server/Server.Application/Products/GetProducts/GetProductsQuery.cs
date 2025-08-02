using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.GetProducts;

public sealed record GetProductsQuery : IQuery<GetProductsResponse>
{
    public int PageSize { get; init; } = 10;
    public string? Cursor { get; init; }
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }

    // Filtering properties
    public string? NameFilter { get; init; }
    public string? DescriptionFilter { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public int? MinStock { get; init; }
    public int? MaxStock { get; init; }
    public string? StatusFilter { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public DateTime? CreatedBefore { get; init; }
    public DateTime? LastUpdatedAfter { get; init; }
    public DateTime? LastUpdatedBefore { get; init; }
    public DateTime? LastRestockedAfter { get; init; }
    public DateTime? LastRestockedBefore { get; init; }
    public bool? HasStock { get; init; }
    public bool? IsReserved { get; init; }
}
 
