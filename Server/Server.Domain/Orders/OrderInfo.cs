namespace Server.Domain.Orders;

public sealed record OrderInfo
{
    private OrderInfo(string value) { Value = value; }

    public string Value { get; init; }

    public static OrderInfo Create(string orderInfo)
    {
        return new OrderInfo(orderInfo);
    }
}
