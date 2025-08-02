using Server.Application.Abstractions.Messaging;
using Server.Application.Users.GetCustomers;

namespace Server.Api.Controllers.Customers;

public sealed record GetCustomersRequest(
    string? Cursor,
    string? SortBy,
    string? SortDirection,
    string? NameFilter,
    string? EmailFilter,
    string? CountryFilter,
    string? CityFilter,
    decimal? MinTotalSpent,
    decimal? MaxTotalSpent,
    int? MinTotalOrders,
    int? MaxTotalOrders,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore,
    int PageSize = 10
) : IQuery<GetCustomersResponse>;
