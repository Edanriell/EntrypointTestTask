namespace Server.Application.Statistics.GetRecentOrders;

public sealed class GetRecentOrdersResponse
{
    public IReadOnlyList<RecentOrder> RecentOrders { get; init; } = [];
}

public sealed class RecentOrder
{
    public string Number { get; init; }
    public string Customer { get; init; }
    public string Status { get; init; }
    public decimal Total { get; init; }
    public DateTime Date { get; init; }
}
