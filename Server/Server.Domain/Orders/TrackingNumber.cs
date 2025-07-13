using Server.Domain.Abstractions;

namespace Server.Domain.Orders;

public sealed record TrackingNumber
{
    private TrackingNumber(string value) { Value = value; }

    public string Value { get; init; }

    public static Result<TrackingNumber> Create(string trackingNumber)
    {
        return new TrackingNumber(trackingNumber);
    }
}
 
