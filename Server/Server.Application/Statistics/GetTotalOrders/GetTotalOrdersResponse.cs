namespace Server.Application.Statistics.GetTotalOrders;

public sealed class GetTotalOrdersResponse
{
    public int TotalOrders { get; init; }
    public double ChangePercent { get; init; }
}
