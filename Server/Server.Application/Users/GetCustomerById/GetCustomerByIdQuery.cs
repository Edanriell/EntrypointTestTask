using Server.Application.Abstractions.Messaging;

namespace Server.Application.Users.GetCustomerById;

// Caching is handled on the FrontEnd side, with TanStack Query help! 
// public sealed record GetCustomerByIdQuery(Guid UserId) : ICachedQuery<GetCustomerByIdResponse>
// {
//     public string CacheKey => $"user-{UserId}";
//     public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
// }

public sealed record GetCustomerByIdQuery(Guid UserId) : IQuery<GetCustomerByIdResponse>
{
}
 
