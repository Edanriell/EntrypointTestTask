namespace Server.Application.Abstractions.Pagination;

public sealed class PagedList<T>
{
    public PagedList(
        List<T> items,
        string? nextCursor,
        string? previousCursor,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        Items = items;
        NextCursor = nextCursor;
        PreviousCursor = previousCursor;
        HasNextPage = hasNextPage;
        HasPreviousPage = hasPreviousPage;
    }

    public List<T> Items { get; }
    public string? NextCursor { get; }
    public string? PreviousCursor { get; }
    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }
}
