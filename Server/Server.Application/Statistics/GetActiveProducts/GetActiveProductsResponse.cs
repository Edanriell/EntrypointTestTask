namespace Server.Application.Statistics.GetActiveProducts;

public sealed class GetActiveProductsResponse
{
    public int ActiveProducts { get; init; }
    public int LowStockProducts { get; init; }
}
