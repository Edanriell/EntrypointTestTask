using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetLowStockAlerts;

public sealed record GetLowStockAlertsQuery : ICachedQuery<GetLowStockAlertsResponse>
{
    public string CacheKey => "low-stock-alerts";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
