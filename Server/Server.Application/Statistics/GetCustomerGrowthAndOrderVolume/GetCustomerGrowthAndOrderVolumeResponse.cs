using Server.Domain.Shared;

namespace Server.Application.Statistics.GetCustomerGrowthAndOrderVolume;

public sealed class GetCustomerGrowthAndOrderVolumeResponse
{
    public IReadOnlyList<CustomerGrowthOrders> Trend { get; init; } = [];
}

public sealed class CustomerGrowthOrders
{
    public Month Month { get; init; }
    public int TotalCustomers { get; init; }
    public int TotalOrders { get; init; }
}
