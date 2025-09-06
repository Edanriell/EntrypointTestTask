using Server.Domain.Abstractions;

namespace Server.Domain.Orders;

public sealed record CancellationReason
{
    private CancellationReason(string value) { Value = value; }

    public string Value { get; init; }

    public static Result<CancellationReason> Create(string value)
    {
        return new CancellationReason(value);
    }
}
