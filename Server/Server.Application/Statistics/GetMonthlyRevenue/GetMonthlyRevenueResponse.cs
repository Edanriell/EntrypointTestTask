namespace Server.Application.Statistics.GetMonthlyRevenue;

public sealed class GetMonthlyRevenueResponse
{
    public decimal TotalRevenue { get; init; }
    public double ChangePercent { get; init; }
}
