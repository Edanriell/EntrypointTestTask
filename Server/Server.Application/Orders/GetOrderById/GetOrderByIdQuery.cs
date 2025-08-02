using Server.Application.Abstractions.Messaging;

namespace Server.Application.Orders.GetOrderById;

// Caching is implemented on client side, using TanStack Query
// public sealed record GetOrderByIdQuery(Guid OrderId) : ICachedQuery<GetOrderByIdResponse>
// {
//     public string CacheKey => $"order-{OrderId}";
//     public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
// }

public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery<GetOrderByIdResponse>
{
}
