using Server.Application.Abstractions.Caching;

namespace Server.Application.Orders.GetOrderByNumber;

public sealed record GetOrderByNumberQuery(Guid OrderNumber) : ICachedQuery<OrdersResponse>
{
    public string CacheKey => $"order-{OrderNumber}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
