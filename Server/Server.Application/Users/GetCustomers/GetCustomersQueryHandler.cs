// Needs refactoring!

using System.Data;
using System.Globalization;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Application.Abstractions.Pagination;
using Server.Domain.Abstractions;

namespace Server.Application.Users.GetCustomers;

internal sealed class GetCustomersQueryHandler : IQueryHandler<GetCustomersQuery, GetCustomersResponse>
{
    private readonly ICursorPaginationService _cursorPaginationService;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCustomersQueryHandler(
        ISqlConnectionFactory sqlConnectionFactory,
        ICursorPaginationService cursorPaginationService
    )
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _cursorPaginationService = cursorPaginationService;
    }

    public async Task<Result<GetCustomersResponse>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        var sqlBuilder = new StringBuilder();
        var parameters = new DynamicParameters();

        sqlBuilder.Append("""
                          SELECT 
                              u.id as Id,
                              u.first_name as FirstName,
                              u.last_name as LastName,
                              u.email as Email,
                              u.phone_number as PhoneNumber,
                              u.gender as Gender,
                              u.address_country as Country,
                              u.address_city as City,
                              u.address_zipcode as ZipCode,
                              u.address_street as Street,
                              u.created_at as CreatedOnUtc,
                              COUNT(DISTINCT o.id) as TotalOrders,
                              COALESCE(SUM(CASE WHEN o.status = 'Completed' THEN 1 ELSE 0 END), 0) as CompletedOrders,
                              COALESCE(SUM(CASE WHEN o.status = 'Pending' THEN 1 ELSE 0 END), 0) as PendingOrders,
                              COALESCE(SUM(CASE WHEN o.status = 'Cancelled' THEN 1 ELSE 0 END), 0) as CancelledOrders,
                              COALESCE(SUM(op.total_price_amount), 0) as TotalSpent,
                              MAX(o.created_at) as LastOrderDate
                          FROM users u
                          INNER JOIN role_user ur ON u.id = ur.users_id
                          INNER JOIN roles r ON ur.roles_id = r.id
                          LEFT JOIN orders o ON u.id = o.client_id
                          LEFT JOIN order_products op ON o.id = op.order_id
                          WHERE r.name = 'Customer'
                          """);

        // Add filtering
        AddFilters(sqlBuilder, parameters, request);

        // Add GROUP BY
        sqlBuilder.Append("""
                           GROUP BY u.id, u.first_name, u.last_name, u.email, u.phone_number, 
                                    u.gender, u.address_country, u.address_city, u.address_zipcode, 
                                    u.address_street, u.created_at
                          """);

        // Add HAVING clause for aggregated filters
        AddHavingFilters(sqlBuilder, parameters, request);

        // Add sorting
        AddSorting(sqlBuilder, request);

        // Add LIMIT and OFFSET for pagination
        CursorInfo cursorInfo = _cursorPaginationService.DecodeCursor(request.Cursor ?? string.Empty);
        int offset = (cursorInfo.PageNumber - 1) * request.PageSize;

        parameters.Add("PageSize", request.PageSize + 1); // +1 to check for next page
        parameters.Add("Offset", offset);
        sqlBuilder.Append(" LIMIT @PageSize OFFSET @Offset");

        var customers = (await connection.QueryAsync<Customer>(sqlBuilder.ToString(), parameters)).ToList();

        // Determine pagination state
        PaginationInfo<Customer> paginationInfo = _cursorPaginationService.DeterminePaginationState(
            customers,
            request.PageSize,
            request.SortBy ?? "CreatedOnUtc",
            cursorInfo.PageNumber,
            customer => GetCustomerSortValue(customer, request.SortBy ?? "CreatedOnUtc"));

        // Get recent orders for each customer
        await LoadRecentOrders(connection, paginationInfo.PageItems);

        // Get total count
        int totalCount = await GetTotalCount(connection, request);

        return new GetCustomersResponse
        {
            Customers = paginationInfo.PageItems,
            NextCursor = paginationInfo.NextCursor,
            PreviousCursor = paginationInfo.PreviousCursor,
            HasNextPage = paginationInfo.HasNextPage,
            HasPreviousPage = paginationInfo.HasPreviousPage,
            TotalCount = totalCount,
            CurrentPageSize = paginationInfo.PageItems.Count,
            PageNumber = _cursorPaginationService.DecodeCursor(request.Cursor ?? string.Empty).PageNumber
        };
    }

    private void AddFilters(StringBuilder sqlBuilder, DynamicParameters parameters, GetCustomersQuery request)
    {
        if (!string.IsNullOrEmpty(request.NameFilter))
        {
            sqlBuilder.Append(" AND (u.first_name ILIKE @NameFilter OR u.last_name ILIKE @NameFilter)");
            parameters.Add("NameFilter", $"%{request.NameFilter}%");
        }

        if (!string.IsNullOrEmpty(request.EmailFilter))
        {
            sqlBuilder.Append(" AND u.email ILIKE @EmailFilter");
            parameters.Add("EmailFilter", $"%{request.EmailFilter}%");
        }

        if (!string.IsNullOrEmpty(request.CountryFilter))
        {
            sqlBuilder.Append(" AND u.address_country ILIKE @CountryFilter");
            parameters.Add("CountryFilter", $"%{request.CountryFilter}%");
        }

        if (!string.IsNullOrEmpty(request.CityFilter))
        {
            sqlBuilder.Append(" AND u.address_city ILIKE @CityFilter");
            parameters.Add("CityFilter", $"%{request.CityFilter}%");
        }

        if (request.CreatedAfter.HasValue)
        {
            sqlBuilder.Append(" AND u.created_at >= @CreatedAfter");
            parameters.Add("CreatedAfter", request.CreatedAfter.Value);
        }

        if (request.CreatedBefore.HasValue)
        {
            sqlBuilder.Append(" AND u.created_at <= @CreatedBefore");
            parameters.Add("CreatedBefore", request.CreatedBefore.Value);
        }
    }

    private void AddHavingFilters(StringBuilder sqlBuilder, DynamicParameters parameters, GetCustomersQuery request)
    {
        var havingConditions = new List<string>();

        if (request.MinTotalSpent.HasValue)
        {
            havingConditions.Add("COALESCE(SUM(op.total_price_amount), 0) >= @MinTotalSpent");
            parameters.Add("MinTotalSpent", request.MinTotalSpent.Value);
        }

        if (request.MaxTotalSpent.HasValue)
        {
            havingConditions.Add("COALESCE(SUM(op.total_price_amount), 0) <= @MaxTotalSpent");
            parameters.Add("MaxTotalSpent", request.MaxTotalSpent.Value);
        }

        if (request.MinTotalOrders.HasValue)
        {
            havingConditions.Add("COUNT(DISTINCT o.id) >= @MinTotalOrders");
            parameters.Add("MinTotalOrders", request.MinTotalOrders.Value);
        }

        if (request.MaxTotalOrders.HasValue)
        {
            havingConditions.Add("COUNT(DISTINCT o.id) <= @MaxTotalOrders");
            parameters.Add("MaxTotalOrders", request.MaxTotalOrders.Value);
        }

        if (havingConditions.Count > 0)
        {
            sqlBuilder.Append(" HAVING ");
            sqlBuilder.Append(string.Join(" AND ", havingConditions));
        }
    }

    private void AddSorting(StringBuilder sqlBuilder, GetCustomersQuery request)
    {
        string sortColumn = GetSortColumn(request.SortBy!);
        string sortDirection = request.SortDirection?.ToUpper() == "ASC" ? "ASC" : "DESC";

        sqlBuilder.Append(CultureInfo.InvariantCulture, $" ORDER BY {sortColumn} {sortDirection}");
    }

    private string GetSortColumn(string sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "firstname" => "u.first_name",
            "lastname" => "u.last_name",
            "email" => "u.email",
            "country" => "u.address_country",
            "city" => "u.address_city",
            "totalorders" => "COUNT(DISTINCT o.id)",
            "completedorders" => "COALESCE(SUM(CASE WHEN o.status = 'Completed' THEN 1 ELSE 0 END), 0)",
            "pendingorders" => "COALESCE(SUM(CASE WHEN o.status = 'Pending' THEN 1 ELSE 0 END), 0)",
            "cancelledorders" => "COALESCE(SUM(CASE WHEN o.status = 'Cancelled' THEN 1 ELSE 0 END), 0)",
            "totalspent" => "COALESCE(SUM(op.total_price_amount), 0)",
            "lastorderdate" => "MAX(o.created_at)",
            "createdonutc" => "u.created_at",
            _ => "u.id"
        };
    }

    private async Task LoadRecentOrders(IDbConnection connection, List<Customer> customers)
    {
        if (customers.Count == 0)
        {
            return;
        }

        var customerIds = customers.Select(c => c.Id).ToList();
        var parameters = new DynamicParameters();
        parameters.Add("CustomerIds", customerIds);

        const string recentOrdersSql = """
                                       SELECT 
                                           o.client_id as UserId,
                                           o.id as OrderId,
                                           COALESCE(SUM(op.total_price_amount), 0) as TotalAmount,
                                           o.status as Status,
                                           o.created_at as CreatedOnUtc
                                       FROM orders o
                                       LEFT JOIN order_products op ON o.id = op.order_id
                                       WHERE o.client_id = ANY(@CustomerIds)
                                       AND o.created_at >= CURRENT_DATE - INTERVAL '30 days'
                                       GROUP BY o.client_id, o.id, o.status, o.created_at
                                       ORDER BY o.created_at DESC
                                       """;

        IEnumerable<RecentOrder> recentOrders = await connection.QueryAsync<RecentOrder>(recentOrdersSql, parameters);

        var recentOrdersByCustomer = recentOrders
            .GroupBy(o => o.UserId)
            .ToDictionary(g => g.Key, g => g.Take(5).ToList());

        foreach (Customer customer in customers)
        {
            customer.RecentOrders = recentOrdersByCustomer.GetValueOrDefault(customer.Id, new List<RecentOrder>());
        }
    }

    private async Task<int> GetTotalCount(IDbConnection connection, GetCustomersQuery request)
    {
        var sqlBuilder = new StringBuilder();
        var parameters = new DynamicParameters();

        sqlBuilder.Append("""
                          SELECT COUNT(DISTINCT u.id)
                          FROM users u
                          INNER JOIN role_user ur ON u.id = ur.users_id
                          INNER JOIN roles r ON ur.roles_id = r.id
                          LEFT JOIN orders o ON u.id = o.client_id
                          LEFT JOIN order_products op ON o.id = op.order_id
                          WHERE r.name = 'Customer'
                          """);

        // Add the same filters as the main query (excluding cursor pagination)
        AddFilters(sqlBuilder, parameters, request);

        // For aggregated filters, we need to wrap in a subquery
        if (HasAggregatedFilters(request))
        {
            string innerQuery = sqlBuilder.ToString();
            sqlBuilder.Clear();
            sqlBuilder.Append("SELECT COUNT(*) FROM (");
            sqlBuilder.Append(innerQuery.Replace("SELECT COUNT(DISTINCT u.id)", "SELECT u.id"));
            sqlBuilder.Append(" GROUP BY u.id");
            AddHavingFilters(sqlBuilder, parameters, request);
            sqlBuilder.Append(") AS filtered_customers");
        }

        return await connection.QuerySingleAsync<int>(sqlBuilder.ToString(), parameters);
    }

    private bool HasAggregatedFilters(GetCustomersQuery request)
    {
        return request.MinTotalSpent.HasValue ||
            request.MaxTotalSpent.HasValue ||
            request.MinTotalOrders.HasValue ||
            request.MaxTotalOrders.HasValue;
    }

    private object GetCustomerSortValue(Customer customer, string sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "firstname" => customer.FirstName,
            "lastname" => customer.LastName,
            "email" => customer.Email,
            "country" => customer.Country,
            "city" => customer.City,
            "totalorders" => customer.TotalOrders,
            "completedorders" => customer.CompletedOrders,
            "pendingorders" => customer.PendingOrders,
            "cancelledorders" => customer.CancelledOrders,
            "totalspent" => customer.TotalSpent,
            "lastorderdate" => customer.LastOrderDate,
            "createdonutc" => customer.CreatedOnUtc,
            _ => customer.Id
        };
    }
}
