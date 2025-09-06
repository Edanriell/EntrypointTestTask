using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetActiveProducts;

public sealed record GetActiveProductsQuery : ICachedQuery<GetActiveProductsResponse>
{
    public string CacheKey => "active-products";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
