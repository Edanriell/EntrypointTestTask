using Server.Application.Abstractions.Messaging;

namespace Server.Application.Users.GetClients;

// public sealed record GetCustomersQuery : IQuery<GetCustomersResponse>;

public sealed class GetCustomersQuery : IQuery<GetCustomersResponse>
{
    public int PageSize { get; init; } = 20;
    public string? Cursor { get; init; }
    public string? SortBy { get; init; } = "CreatedOnUtc";
    public string? SortDirection { get; init; } = "DESC";

    // Filtering properties
    public string? NameFilter { get; init; }
    public string? EmailFilter { get; init; }
    public string? CountryFilter { get; init; }
    public string? CityFilter { get; init; }
    public decimal? MinTotalSpent { get; init; }
    public decimal? MaxTotalSpent { get; init; }
    public int? MinTotalOrders { get; init; }
    public int? MaxTotalOrders { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public DateTime? CreatedBefore { get; init; }
}
