namespace Server.Api.Controllers.Products;

public sealed record GetProductsRequest(
    string? Cursor,
    string? SortBy,
    string? SortDirection,
    string? NameFilter,
    string? DescriptionFilter,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinStock,
    int? MaxStock,
    string? StatusFilter,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore,
    DateTime? LastUpdatedAfter,
    DateTime? LastUpdatedBefore,
    DateTime? LastRestockedAfter,
    DateTime? LastRestockedBefore,
    bool? HasStock,
    bool? IsReserved,
    int PageSize = 10);
 
