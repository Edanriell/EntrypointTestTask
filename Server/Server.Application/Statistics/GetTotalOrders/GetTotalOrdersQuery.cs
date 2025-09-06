using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetTotalOrders;

public sealed record GetTotalOrdersQuery : ICachedQuery<GetTotalOrdersResponse>
{
    public string CacheKey => "total-orders";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
