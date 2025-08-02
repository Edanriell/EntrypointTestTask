using Server.Application.Abstractions.Messaging;

namespace Server.Application.Products.GetProductById;

// Caching is handled on the client side via TanStack Query
// public sealed record GetProductByIdQuery(Guid ProductId) : ICachedQuery<GetProductByIdResponse>
// {
//     public string CacheKey => $"product-{ProductId}";
//     public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
// }

public sealed record GetProductByIdQuery(Guid ProductId) : IQuery<GetProductByIdResponse>
{
}
