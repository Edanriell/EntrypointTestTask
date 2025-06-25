using Server.Application.Abstractions.Caching;

namespace Server.Application.Products.GetProductById;

public sealed record GetProductByIdQuery(Guid ProductId) : ICachedQuery<ProductResponse>
{
    public string CacheKey => $"product-{ProductId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}
