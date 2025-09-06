using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetOrderStatusDistribution;

public sealed record GetOrderStatusDistributionQuery : ICachedQuery<GetOrderStatusDistributionResponse>
{
    public string CacheKey => "order-status-distribution";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
