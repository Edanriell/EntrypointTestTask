using Server.Application.Abstractions.Caching;

namespace Server.Application.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : ICachedQuery<OrdersResponse>
{
    public string CacheKey => $"order-{OrderId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
