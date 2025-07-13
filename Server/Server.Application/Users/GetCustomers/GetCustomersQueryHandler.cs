using System.Data;
using System.Globalization;
using System.Text;
using Dapper;
using Server.Application.Abstractions.Data;
using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;

namespace Server.Application.Users.GetClients;

internal sealed class GetCustomersQueryHandler : IQueryHandler<GetCustomersQuery, GetCustomersResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCustomersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<GetCustomersResponse>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = _sqlConnectionFactory.CreateConnection();

        var sqlBuilder = new StringBuilder();
        var parameters = new DynamicParameters();

        // Base query
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

        // Add cursor pagination
        AddCursorPagination(sqlBuilder, parameters, request);

        // Add sorting
        AddSorting(sqlBuilder, request);

        // Add LIMIT - get extra records to check for previous/next pages
        parameters.Add("PageSize", request.PageSize + 1); // +1 to check for next page
        sqlBuilder.Append(" LIMIT @PageSize");

        var customers = (await connection.QueryAsync<Customer>(sqlBuilder.ToString(), parameters)).ToList();

        // Calculate current page number
        int currentPage = CalculatePageNumber(request.Cursor);

        // Determine pagination state
        PaginationInfo paginationInfo = DeterminePaginationState(customers, request, currentPage);

        // Get recent orders for each customer
        await LoadRecentOrders(connection, paginationInfo.PageCustomers);

        // Get total count
        int totalCount = await GetTotalCount(connection, request);

        return new GetCustomersResponse
        {
            Customers = paginationInfo.PageCustomers,
            NextCursor = paginationInfo.NextCursor,
            PreviousCursor = paginationInfo.PreviousCursor,
            HasNextPage = paginationInfo.HasNextPage,
            HasPreviousPage = paginationInfo.HasPreviousPage,
            TotalCount = totalCount,
            CurrentPageSize = paginationInfo.PageCustomers.Count,
            PageNumber = currentPage
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

    private void AddCursorPagination(StringBuilder sqlBuilder, DynamicParameters parameters, GetCustomersQuery request)
    {
        if (!string.IsNullOrEmpty(request.Cursor))
        {
            CursorInfo cursorData = DecodeCursor(request.Cursor);
            string sortDirection = request.SortDirection?.ToUpper() == "ASC" ? ">" : "<";

            sqlBuilder.Append(CultureInfo.InvariantCulture,
                $" AND u.{GetSortColumn(request.SortBy)} {sortDirection} @CursorValue");
            parameters.Add("CursorValue", cursorData.Value);
        }
    }

    private void AddSorting(StringBuilder sqlBuilder, GetCustomersQuery request)
    {
        string sortColumn = GetSortColumn(request.SortBy);
        string sortDirection = request.SortDirection?.ToUpper() == "ASC" ? "ASC" : "DESC";

        sqlBuilder.Append(CultureInfo.InvariantCulture, $" ORDER BY u.{sortColumn} {sortDirection}");
    }

    private string GetSortColumn(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "firstname" => "first_name",
            "lastname" => "last_name",
            "email" => "email",
            "createdonutc" => "created_at",
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
                    SortBy = "createdonutc",
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

    private string GenerateCursor(Customer customer, string? sortBy, int pageNumber, int position)
    {
        string actualSortBy = sortBy ?? "createdonutc";

        string cursorValue = actualSortBy.ToLower() switch
        {
            "firstname" => customer.FirstName,
            "lastname" => customer.LastName,
            "email" => customer.Email,
            "createdonutc" => customer.CreatedOnUtc.ToString("O"),
            _ => customer.CreatedOnUtc.ToString("O")
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
        List<Customer> customers, GetCustomersQuery request, int currentPage)
    {
        bool hasNextPage = customers.Count > request.PageSize;
        bool hasPreviousPage = currentPage > 1;

        // Remove extra records used for pagination detection
        var pageCustomers = customers.Take(request.PageSize).ToList();

        string? nextCursor = null;
        string? previousCursor = null;

        if (hasNextPage && pageCustomers.Count > 0)
        {
            int nextPageNumber = currentPage + 1;
            int nextPosition = (currentPage - 1) * request.PageSize + pageCustomers.Count;
            nextCursor = GenerateCursor(pageCustomers[^1], request.SortBy, nextPageNumber, nextPosition);
        }

        if (hasPreviousPage && pageCustomers.Count > 0)
        {
            int previousPageNumber = Math.Max(1, currentPage - 1);
            int previousPosition = Math.Max(0, (previousPageNumber - 1) * request.PageSize);
            previousCursor = GenerateCursor(pageCustomers[0], request.SortBy, previousPageNumber, previousPosition);
        }

        return new PaginationInfo
        {
            PageCustomers = pageCustomers,
            NextCursor = nextCursor,
            PreviousCursor = previousCursor,
            HasNextPage = hasNextPage,
            HasPreviousPage = hasPreviousPage
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
                                           o.client_id as ClientId,
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

    private sealed record CursorInfo
    {
        public string Value { get; init; } = string.Empty;
        public string SortBy { get; init; } = string.Empty;
        public int PageNumber { get; init; } = 1;
        public int Position { get; init; }
    }

    private sealed record PaginationInfo
    {
        public List<Customer> PageCustomers { get; init; } = new();
        public string? NextCursor { get; init; }
        public string? PreviousCursor { get; init; }
        public bool HasNextPage { get; init; }
        public bool HasPreviousPage { get; init; }
    }
}
