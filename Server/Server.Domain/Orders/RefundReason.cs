using Server.Domain.Abstractions;

namespace Server.Domain.Orders;

public sealed record RefundReason
{
    private RefundReason(string value) { Value = value; }

    public string Value { get; init; }

    public static Result<RefundReason> Create(string refundReason)
    {
        return new RefundReason(refundReason);
    }
}
 
