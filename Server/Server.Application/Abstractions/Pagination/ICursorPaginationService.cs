namespace Server.Application.Abstractions.Pagination;

public interface ICursorPaginationService
{
    string EncodeCursor(CursorInfo cursorInfo);
    CursorInfo DecodeCursor(string cursor);

    PaginationInfo<T> DeterminePaginationState<T>(
        List<T> items,
        int requestedPageSize,
        string? sortBy = null,
        int? currentPageNumber = null,
        Func<T, object>? sortValueExtractor = null);

    bool IsValidCursor(string cursor);
}
 
