using Server.Domain.Orders;

namespace Server.Application.Statistics.GetOrderStatusDistribution;

public sealed class GetOrderStatusDistributionResponse
{
    public IReadOnlyList<OrderStatusDistribution> OrderStatusDistributions { get; init; } = [];
}

public sealed class OrderStatusDistribution
{
    public OrderStatus Status { get; init; }
    public int Count { get; init; }
}
