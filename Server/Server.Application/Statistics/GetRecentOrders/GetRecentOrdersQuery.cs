using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetRecentOrders;

public sealed record GetRecentOrdersQuery : ICachedQuery<GetRecentOrdersResponse>
{
    public string CacheKey => "recent-response";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
