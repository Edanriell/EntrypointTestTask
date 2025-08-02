using System.Reflection;
using System.Text;
using System.Text.Json;
using Server.Application.Abstractions.Pagination;

namespace Server.Infrastructure.Pagination;

public sealed class CursorPaginationService : ICursorPaginationService
{
    public string EncodeCursor(CursorInfo cursorInfo)
    {
        string json = JsonSerializer.Serialize(cursorInfo);
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    public CursorInfo DecodeCursor(string cursor)
    {
        if (string.IsNullOrEmpty(cursor))
        {
            return new CursorInfo();
        }

        try
        {
            byte[] bytes = Convert.FromBase64String(cursor);
            string json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<CursorInfo>(json) ?? new CursorInfo();
        }
        catch (Exception)
        {
            return new CursorInfo();
        }
    }

    public PaginationInfo<T> DeterminePaginationState<T>(
        List<T> items,
        int requestedPageSize,
        string? sortBy = null,
        int? currentPageNumber = null,
        Func<T, object>? sortValueExtractor = null)
    {
        bool hasNextPage = items.Count > requestedPageSize;
        bool hasPreviousPage = currentPageNumber.HasValue && currentPageNumber.Value > 1;

        // Remove the extra item used for next page detection
        List<T> pageItems = hasNextPage ? items.Take(requestedPageSize).ToList() : items;

        string? nextCursor = null;
        string? previousCursor = null;

        if (hasNextPage && pageItems.Count > 0)
        {
            T lastItem = pageItems[^1];
            var nextCursorInfo = new CursorInfo
            {
                Value = ExtractCursorValue(lastItem, sortBy, sortValueExtractor),
                SortBy = sortBy ?? "Id",
                PageNumber = (currentPageNumber ?? 1) + 1,
                Position = requestedPageSize
            };
            nextCursor = EncodeCursor(nextCursorInfo);
        }

        if (hasPreviousPage && pageItems.Count > 0)
        {
            T firstItem = pageItems[0];
            var previousCursorInfo = new CursorInfo
            {
                Value = ExtractCursorValue(firstItem, sortBy, sortValueExtractor),
                SortBy = sortBy ?? "Id",
                PageNumber = (currentPageNumber ?? 1) - 1,
                Position = 0
            };
            previousCursor = EncodeCursor(previousCursorInfo);
        }

        return new PaginationInfo<T>
        {
            PageItems = pageItems,
            NextCursor = nextCursor,
            PreviousCursor = previousCursor,
            HasNextPage = hasNextPage,
            HasPreviousPage = hasPreviousPage
        };
    }

    public bool IsValidCursor(string cursor)
    {
        if (string.IsNullOrEmpty(cursor))
        {
            return false;
        }

        try
        {
            byte[] bytes = Convert.FromBase64String(cursor);
            string json = Encoding.UTF8.GetString(bytes);
            CursorInfo? cursorInfo = JsonSerializer.Deserialize<CursorInfo>(json);
            return cursorInfo != null;
        }
        catch
        {
            return false;
        }
    }

    private string ExtractCursorValue<T>(T item, string? sortBy, Func<T, object>? sortValueExtractor)
    {
        if (sortValueExtractor != null)
        {
            return sortValueExtractor(item)?.ToString() ?? string.Empty;
        }

        if (string.IsNullOrEmpty(sortBy))
        {
            sortBy = "Id";
        }

        // Use reflection to get the property value
        PropertyInfo? property = typeof(T).GetProperty(sortBy);
        if (property != null)
        {
            object? value = property.GetValue(item);
            return value?.ToString() ?? string.Empty;
        }

        return string.Empty;
    }
}
