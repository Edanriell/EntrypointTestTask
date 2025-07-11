// using System.Data;
// using Dapper;
// using Server.Application.Abstractions.Data;
// using Server.Application.Abstractions.Messaging;
// using Server.Domain.Abstractions;
//
// namespace Server.Application.Products.GetProducts;
//
// internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductsResponse>>
// {
//     private readonly ISqlConnectionFactory _sqlConnectionFactory;
//
//     public GetProductsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
//     {
//         _sqlConnectionFactory = sqlConnectionFactory;
//     }
//
//     public async Task<Result<IReadOnlyList<ProductsResponse>>> Handle(
//         GetProductsQuery request,
//         CancellationToken cancellationToken)
//     {
//         using IDbConnection connection = _sqlConnectionFactory.CreateConnection();
//
//         const string sql = """
//                             SELECT
//                                 id AS Id,
//                                 name As Name,
//                                 description AS Description,
//                                 price_amount AS Price,
//                                 reserved AS Reserved,
//                                 stock AS Stock,
//                                 status AS Status,
//                                 created_at AS CreatedAt,
//                                 last_updated_at AS LastUpdatedAt,
//                                 last_restocked_at AS LastRestockedAt
//                             FROM products
//                            """;
//
//         IEnumerable<ProductsResponse> products = await connection.QueryAsync<ProductsResponse>(sql);
//
//         return products.ToList();
//     }
// }

using System.Data;
using System.Globalization;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Products.GetProducts;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetProductsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetProductsResponse>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        var sqlBuilder = new StringBuilder();
        var parameters = new DynamicParameters();

        // Base query
        sqlBuilder.Append("""
                          SELECT 
                              p.id as Id,
                              p.name as Name,
                              p.description as Description,
                              p.price_amount as Price,
                              p.reserved as Reserved,
                              p.stock as Stock,
                              p.status as Status,
                              p.created_at as CreatedAt,
                              p.last_updated_at as LastUpdatedAt,
                              p.last_restocked_at as LastRestockedAt
                          FROM products p
                          WHERE 1=1
                          """);

        // Add filtering
        AddFilters(sqlBuilder, parameters, request);

        // Add cursor pagination
        AddCursorPagination(sqlBuilder, parameters, request);

        // Add sorting
        AddSorting(sqlBuilder, request);

        // Add LIMIT - get extra records to check for previous/next pages
        parameters.Add("PageSize", request.PageSize + 1); // +1 to check for next page
        sqlBuilder.Append(" LIMIT @PageSize");

        var products = (await connection.QueryAsync<ProductsResponse>(sqlBuilder.ToString(), parameters)).ToList();

        // Calculate current page number
        int currentPage = CalculatePageNumber(request.Cursor);

        // Determine pagination state
        PaginationInfo paginationInfo = DeterminePaginationState(products, request, currentPage);

        // Get total count
        int totalCount = await GetTotalCount(connection, request);

        return new GetProductsResponse
        {
            Products = paginationInfo.PageProducts,
            NextCursor = paginationInfo.NextCursor,
            PreviousCursor = paginationInfo.PreviousCursor,
            HasNextPage = paginationInfo.HasNextPage,
            HasPreviousPage = paginationInfo.HasPreviousPage,
            TotalCount = totalCount,
            CurrentPageSize = paginationInfo.PageProducts.Count,
            PageNumber = currentPage
        };
    }

    private void AddFilters(StringBuilder sqlBuilder, DynamicParameters parameters, GetProductsQuery request)
    {
        if (!string.IsNullOrEmpty(request.NameFilter))
        {
            sqlBuilder.Append(" AND p.name ILIKE @NameFilter");
            parameters.Add("NameFilter", $"%{request.NameFilter}%");
        }

        if (!string.IsNullOrEmpty(request.DescriptionFilter))
        {
            sqlBuilder.Append(" AND p.description ILIKE @DescriptionFilter");
            parameters.Add("DescriptionFilter", $"%{request.DescriptionFilter}%");
        }

        if (request.MinPrice.HasValue)
        {
            sqlBuilder.Append(" AND p.price_amount >= @MinPrice");
            parameters.Add("MinPrice", request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            sqlBuilder.Append(" AND p.price_amount <= @MaxPrice");
            parameters.Add("MaxPrice", request.MaxPrice.Value);
        }

        if (request.MinStock.HasValue)
        {
            sqlBuilder.Append(" AND p.stock >= @MinStock");
            parameters.Add("MinStock", request.MinStock.Value);
        }

        if (request.MaxStock.HasValue)
        {
            sqlBuilder.Append(" AND p.stock <= @MaxStock");
            parameters.Add("MaxStock", request.MaxStock.Value);
        }

        if (!string.IsNullOrEmpty(request.StatusFilter))
        {
            sqlBuilder.Append(" AND p.status ILIKE @StatusFilter");
            parameters.Add("StatusFilter", $"%{request.StatusFilter}%");
        }

        if (request.CreatedAfter.HasValue)
        {
            sqlBuilder.Append(" AND p.created_at >= @CreatedAfter");
            parameters.Add("CreatedAfter", request.CreatedAfter.Value);
        }

        if (request.CreatedBefore.HasValue)
        {
            sqlBuilder.Append(" AND p.created_at <= @CreatedBefore");
            parameters.Add("CreatedBefore", request.CreatedBefore.Value);
        }

        if (request.LastUpdatedAfter.HasValue)
        {
            sqlBuilder.Append(" AND p.last_updated_at >= @LastUpdatedAfter");
            parameters.Add("LastUpdatedAfter", request.LastUpdatedAfter.Value);
        }

        if (request.LastUpdatedBefore.HasValue)
        {
            sqlBuilder.Append(" AND p.last_updated_at <= @LastUpdatedBefore");
            parameters.Add("LastUpdatedBefore", request.LastUpdatedBefore.Value);
        }

        if (request.LastRestockedAfter.HasValue)
        {
            sqlBuilder.Append(" AND p.last_restocked_at >= @LastRestockedAfter");
            parameters.Add("LastRestockedAfter", request.LastRestockedAfter.Value);
        }

        if (request.LastRestockedBefore.HasValue)
        {
            sqlBuilder.Append(" AND p.last_restocked_at <= @LastRestockedBefore");
            parameters.Add("LastRestockedBefore", request.LastRestockedBefore.Value);
        }

        if (request.HasStock.HasValue)
        {
            if (request.HasStock.Value)
            {
                sqlBuilder.Append(" AND p.stock > 0");
            }
            else
            {
                sqlBuilder.Append(" AND p.stock = 0");
            }
        }

        if (request.IsReserved.HasValue)
        {
            if (request.IsReserved.Value)
            {
                sqlBuilder.Append(" AND p.reserved > 0");
            }
            else
            {
                sqlBuilder.Append(" AND p.reserved = 0");
            }
        }
    }

    private void AddCursorPagination(StringBuilder sqlBuilder, DynamicParameters parameters, GetProductsQuery request)
    {
        if (!string.IsNullOrEmpty(request.Cursor))
        {
            CursorInfo cursorData = DecodeCursor(request.Cursor);
            string sortDirection = request.SortDirection?.ToUpper() == "ASC" ? ">" : "<";

            sqlBuilder.Append(CultureInfo.InvariantCulture,
                $" AND p.{GetSortColumn(request.SortBy)} {sortDirection} @CursorValue");
            parameters.Add("CursorValue", cursorData.Value);
        }
    }

    private void AddSorting(StringBuilder sqlBuilder, GetProductsQuery request)
    {
        string sortColumn = GetSortColumn(request.SortBy);
        string sortDirection = request.SortDirection?.ToUpper() == "ASC" ? "ASC" : "DESC";

        sqlBuilder.Append(CultureInfo.InvariantCulture, $" ORDER BY p.{sortColumn} {sortDirection}");
    }

    private string GetSortColumn(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "name" => "name",
            "price" => "price_amount",
            "stock" => "stock",
            "reserved" => "reserved",
            "status" => "status",
            "createdat" => "created_at",
            "lastupdatedat" => "last_updated_at",
            "lastrestockedat" => "last_restocked_at",
            _ => "created_at"
        };
    }

    private CursorInfo DecodeCursor(string cursor)
    {
        try
        {
            string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            string[] parts = decoded.Split('|');

            return parts.Length switch
            {
                >= 4 => new CursorInfo
                {
                    Value = parts[0],
                    SortBy = parts[1],
                    PageNumber = int.Parse(parts[2], CultureInfo.InvariantCulture),
                    Position = int.Parse(parts[3], CultureInfo.InvariantCulture)
                },
                >= 2 => new CursorInfo
                {
                    Value = parts[0],
                    SortBy = parts[1],
                    PageNumber = 1,
                    Position = 0
                },
                _ => new CursorInfo
                {
                    Value = decoded,
                    SortBy = "createdat",
                    PageNumber = 1,
                    Position = 0
                }
            };
        }
        catch
        {
            throw new ArgumentException("Invalid cursor format");
        }
    }

    private string GenerateCursor(ProductsResponse product, string? sortBy, int pageNumber, int position)
    {
        string actualSortBy = sortBy ?? "createdat";

        string cursorValue = actualSortBy.ToLower() switch
        {
            "name" => product.Name,
            "price" => product.Price.ToString(CultureInfo.InvariantCulture),
            "stock" => product.Stock.ToString(CultureInfo.InvariantCulture),
            "reserved" => product.Reserved.ToString(CultureInfo.InvariantCulture),
            "status" => product.Status,
            "createdat" => product.CreatedAt.ToString("O"),
            "lastupdatedat" => product.LastUpdatedAt.ToString("O"),
            "lastrestockedat" => product.LastRestockedAt?.ToString("O") ?? DateTime.MinValue.ToString("O"),
            _ => product.CreatedAt.ToString("O")
        };

        // Include page number and position in cursor using InvariantCulture
        string cursorData =
            $"{cursorValue}|{actualSortBy}|{pageNumber.ToString(CultureInfo.InvariantCulture)}|{position.ToString(CultureInfo.InvariantCulture)}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(cursorData));
    }

    private int CalculatePageNumber(string? cursor)
    {
        if (string.IsNullOrEmpty(cursor))
        {
            return 1;
        }

        try
        {
            CursorInfo cursorInfo = DecodeCursor(cursor);
            return cursorInfo.PageNumber;
        }
        catch
        {
            return 1;
        }
    }

    private PaginationInfo DeterminePaginationState(
        List<ProductsResponse> products, GetProductsQuery request, int currentPage)
    {
        bool hasNextPage = products.Count > request.PageSize;
        bool hasPreviousPage = currentPage > 1;

        // Remove extra records used for pagination detection
        var pageProducts = products.Take(request.PageSize).ToList();

        string? nextCursor = null;
        string? previousCursor = null;

        if (hasNextPage && pageProducts.Count > 0)
        {
            int nextPageNumber = currentPage + 1;
            int nextPosition = (currentPage - 1) * request.PageSize + pageProducts.Count;
            nextCursor = GenerateCursor(pageProducts[^1], request.SortBy, nextPageNumber, nextPosition);
        }

        if (hasPreviousPage && pageProducts.Count > 0)
        {
            int previousPageNumber = Math.Max(1, currentPage - 1);
            int previousPosition = Math.Max(0, (previousPageNumber - 1) * request.PageSize);
            previousCursor = GenerateCursor(pageProducts[0], request.SortBy, previousPageNumber, previousPosition);
        }

        return new PaginationInfo
        {
            PageProducts = pageProducts,
            NextCursor = nextCursor,
            PreviousCursor = previousCursor,
            HasNextPage = hasNextPage,
            HasPreviousPage = hasPreviousPage
        };
    }

    private async Task<int> GetTotalCount(IDbConnection connection, GetProductsQuery request)
    {
        var sqlBuilder = new StringBuilder();
        var parameters = new DynamicParameters();

        sqlBuilder.Append("""
                          SELECT COUNT(*)
                          FROM products p
                          WHERE 1=1
                          """);

        // Add the same filters as the main query (excluding cursor pagination)
        AddFilters(sqlBuilder, parameters, request);

        return await connection.QuerySingleAsync<int>(sqlBuilder.ToString(), parameters);
    }

    private sealed record CursorInfo
    {
        public string Value { get; init; } = string.Empty;
        public string SortBy { get; init; } = string.Empty;
        public int PageNumber { get; init; } = 1;
        public int Position { get; init; }
    }

    private sealed record PaginationInfo
    {
        public List<ProductsResponse> PageProducts { get; init; } = new();
        public string? NextCursor { get; init; }
        public string? PreviousCursor { get; init; }
        public bool HasNextPage { get; init; }
        public bool HasPreviousPage { get; init; }
    }
}
