using Server.Application.Abstractions.Clock;

namespace Server.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
} 
 
 
