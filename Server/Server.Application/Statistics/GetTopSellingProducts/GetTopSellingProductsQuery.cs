using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetTopSellingProducts;

public sealed record GetTopSellingProductsQuery : ICachedQuery<GetTopSellingProductsResponse>
{
    public string CacheKey => "top-selling-products";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
