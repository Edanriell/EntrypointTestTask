namespace Server.Application.Statistics.GetLowStockAlerts;

public sealed class GetLowStockAlertsResponse
{
    public IReadOnlyList<LowStockProduct> LowStockProducts { get; init; } = [];
}

public sealed class LowStockProduct
{
    public string ProductName { get; init; }
    public int UnitsInStock { get; init; }
}
