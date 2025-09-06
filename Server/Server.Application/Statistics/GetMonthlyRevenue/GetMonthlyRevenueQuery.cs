using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetMonthlyRevenue;

public sealed record GetMonthlyRevenueQuery : ICachedQuery<GetMonthlyRevenueResponse>
{
    public string CacheKey => "monthly-revenue";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
