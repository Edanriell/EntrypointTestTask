namespace Server.Application.Abstractions.Pagination;

public sealed record PaginationInfo<T>
{
    public List<T> PageItems { get; init; } = new();
    public string? NextCursor { get; init; }
    public string? PreviousCursor { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
}
 
