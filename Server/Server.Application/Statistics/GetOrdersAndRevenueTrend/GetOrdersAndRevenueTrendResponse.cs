using Server.Domain.Shared;

namespace Server.Application.Statistics.GetOrdersAndRevenueTrend;

public sealed class GetOrdersAndRevenueTrendResponse
{
    public IReadOnlyList<MonthlyOrdersRevenue> Trend { get; init; } = [];
}

public sealed class MonthlyOrdersRevenue
{
    public Month Month { get; init; }
    public int Orders { get; init; }
    public int Revenue { get; init; }
}
