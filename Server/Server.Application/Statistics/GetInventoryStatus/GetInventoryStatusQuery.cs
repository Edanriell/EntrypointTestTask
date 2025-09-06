using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetInventoryStatus;

public sealed record GetInventoryStatusQuery : ICachedQuery<GetInventoryStatusResponse>
{
    public string CacheKey => "inventory-status";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
