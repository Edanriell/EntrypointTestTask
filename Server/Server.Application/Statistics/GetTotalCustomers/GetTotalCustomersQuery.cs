using Server.Application.Abstractions.Caching;

namespace Server.Application.Statistics.GetTotalCustomers;

public sealed record GetTotalCustomersQuery : ICachedQuery<GetTotalCustomersResponse>
{
    public string CacheKey => "total-customers";
    public TimeSpan? Expiration => TimeSpan.FromMilliseconds(5);
}
