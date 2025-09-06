using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetOrdersAndRevenueTrend;

public sealed record GetOrdersAndRevenueTrendQuery : ICachedQuery<GetOrdersAndRevenueTrendResponse>
{
    public string CacheKey => "revenue-trend";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
