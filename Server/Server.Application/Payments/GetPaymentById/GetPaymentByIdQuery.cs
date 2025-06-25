using Server.Application.Abstractions.Caching;

namespace Server.Application.Payments.GetPaymentById;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : ICachedQuery<PaymentResponse>
{
    public string CacheKey => $"payment-{PaymentId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
