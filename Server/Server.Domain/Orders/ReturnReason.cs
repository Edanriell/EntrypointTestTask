namespace Server.Domain.Orders;

public sealed record ReturnReason
{
    private ReturnReason(string value) { Value = value; }

    public string Value { get; init; }

    public static ReturnReason Create(string returnReason)
    {
        return new ReturnReason(returnReason);
    }
}
