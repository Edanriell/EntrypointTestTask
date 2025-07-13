using Server.Application.Abstractions.Caching;

namespace Server.Application.Users.GetUserById;

public sealed record GetCustomerByIdQuery(Guid UserId) : ICachedQuery<CustomerResponse>
{
    public string CacheKey => $"user-{UserId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
}
 
