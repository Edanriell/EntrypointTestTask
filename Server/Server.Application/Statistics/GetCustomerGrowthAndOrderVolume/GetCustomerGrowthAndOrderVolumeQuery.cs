using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetCustomerGrowthAndOrderVolume;

public sealed record GetCustomerGrowthAndOrderVolumeQuery : ICachedQuery<GetCustomerGrowthAndOrderVolumeResponse>
{
    public string CacheKey => "customer-growth-orders";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
