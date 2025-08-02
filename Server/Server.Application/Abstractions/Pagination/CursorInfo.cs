namespace Server.Application.Abstractions.Pagination;

public sealed record CursorInfo
{
    public string Value { get; init; } = string.Empty;
    public string SortBy { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int Position { get; init; }
}
