namespace Server.Application.Statistics.GetTopSellingProducts;

public sealed class GetTopSellingProductsResponse
{
    public IReadOnlyList<BestSellingProduct> BestSellingProducts { get; init; } = [];
}

public sealed class BestSellingProduct
{
    public string ProductName { get; init; } = string.Empty;
    public int UnitsSold { get; init; }
    public int Revenue { get; init; }
}
