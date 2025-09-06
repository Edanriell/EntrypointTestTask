// Needs refactoring!

using System.Data;
using System.Globalization;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Application.Abstractions.Pagination;
using Server.Domain.Abstractions;

namespace Server.Application.Products.GetProducts;

internal sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsResponse>
{
    private readonly ICursorPaginationService _cursorPaginationService;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetProductsQueryHandler(
        ISqlConnectionFactory sqlConnectionFactory,
        ICursorPaginationService cursorPaginationService)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _cursorPaginationService = cursorPaginationService;
    }

    public async Task<Result<GetProductsResponse>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        var sqlBuilder = new StringBuilder();
        var parameters = new DynamicParameters();

        sqlBuilder.Append("""
                          SELECT 
                              p.id as Id,
                              p.name as Name,
                              p.description as Description,
                              p.price_amount as Price,
                              p.price_currency as Currency,
                              p.total_stock AS TotalStock,
                              p.reserved AS Reserved,
                              (p.total_stock - p.reserved) as Available,
                              (p.total_stock - p.reserved = 0) as IsOutOfStock,
                              (p.total_stock - p.reserved <= 25) as HasLowStock,
                              (p.reserved > 0) as HasReservations,
                              (p.total_stock - p.reserved > 0) as IsInStock,
                              p.status as Status,
                              p.created_at as CreatedAt,
                              p.last_updated_at as LastUpdatedAt,
                              p.last_restocked_at as LastRestockedAt
                          FROM products p
                          WHERE 1=1
                          """);

        // Add filtering
        AddFilters(sqlBuilder, parameters, request);

        // Add sorting
        AddSorting(sqlBuilder, request);

        CursorInfo cursorInfo = _cursorPaginationService.DecodeCursor(request.Cursor ?? string.Empty);
        int offset = (cursorInfo.PageNumber - 1) * request.PageSize;

        parameters.Add("PageSize", request.PageSize + 1); // +1 to check for next page
        parameters.Add("Offset", offset);
        sqlBuilder.Append(" LIMIT @PageSize OFFSET @Offset");

        var products = (await connection.QueryAsync<GetAllProductsResponse>(sqlBuilder.ToString(), parameters))
            .ToList();

        // Determine pagination state
        PaginationInfo<GetAllProductsResponse> paginationInfo = _cursorPaginationService.DeterminePaginationState(
            products,
            request.PageSize,
            request.SortBy ?? "CreatedOnUtc",
            cursorInfo.PageNumber,
            product => GetProductSortValue(product, request.SortBy ?? "CreatedOnUtc"));

        int totalCount = await GetTotalCount(connection, request);

        return new GetProductsResponse
        {
            Products = paginationInfo.PageItems,
            NextCursor = paginationInfo.NextCursor,
            PreviousCursor = paginationInfo.PreviousCursor,
            HasNextPage = paginationInfo.HasNextPage,
            HasPreviousPage = paginationInfo.HasPreviousPage,
            TotalCount = totalCount,
            CurrentPageSize = paginationInfo.PageItems.Count,
            PageNumber = _cursorPaginationService.DecodeCursor(request.Cursor ?? string.Empty).PageNumber
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
            sqlBuilder.Append(" AND p.total_stock >= @MinStock");
            parameters.Add("MinStock", request.MinStock.Value);
        }

        if (request.MaxStock.HasValue)
        {
            sqlBuilder.Append(" AND p.total_stock <= @MaxStock");
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
                sqlBuilder.Append(" AND p.total_stock > 0");
            }
            else
            {
                sqlBuilder.Append(" AND p.total_stock = 0");
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

        if (request.HasLowStock.HasValue)
        {
            if (request.HasLowStock.Value)
            {
                sqlBuilder.Append(" AND p.total_stock - p.reserved <= 25");
            }
            else
            {
                sqlBuilder.Append(" AND p.total_stock - p.reserved > 25");
            }
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
            "totalstock" => "total_stock",
            "reserved" => "reserved",
            "status" => "status",
            "createdat" => "created_at",
            "lastupdatedat" => "last_updated_at",
            "lastrestockedat" => "last_restocked_at",
            _ => "id"
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
                          AND status != 'Deleted'
                          """);

        // Add the same filters as the main query (excluding cursor pagination)
        AddFilters(sqlBuilder, parameters, request);

        return await connection.QuerySingleAsync<int>(sqlBuilder.ToString(), parameters);
    }

    private object GetProductSortValue(GetAllProductsResponse getAllProduct, string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "name" => getAllProduct.Name,
            "price" => getAllProduct.Price,
            "total_stock" => getAllProduct.TotalStock,
            "reserved" => getAllProduct.Reserved,
            "status" => getAllProduct.Status,
            "createdat" => getAllProduct.CreatedAt,
            "lastupdatedat" => getAllProduct.LastUpdatedAt,
            "lastrestockedat" => getAllProduct.LastRestockedAt,
            _ => getAllProduct.Id
        };
    }
}
