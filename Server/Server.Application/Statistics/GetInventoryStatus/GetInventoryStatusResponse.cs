using Server.Domain.Shared;

namespace Server.Application.Statistics.GetInventoryStatus;

public sealed class GetInventoryStatusResponse
{
    public IReadOnlyList<ProductInventory> InventorySummary { get; init; } = [];
}

public sealed class ProductInventory
{
    public InventoryStatus InventoryStatus { get; init; }
    public int Count { get; init; }
}
