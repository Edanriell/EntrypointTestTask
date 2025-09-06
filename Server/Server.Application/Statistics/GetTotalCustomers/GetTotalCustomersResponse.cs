namespace Server.Application.Statistics.GetTotalCustomers;

public sealed class GetTotalCustomersResponse
{
    public int TotalCustomers { get; init; }
    public int NewThisMonth { get; init; }
}
