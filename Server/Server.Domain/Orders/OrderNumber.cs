using Server.Domain.Abstractions;

namespace Server.Domain.Orders;
 
public sealed record OrderNumber
{
    private OrderNumber(string value) { Value = value; }

    public string Value { get; init; }

    public static Result<OrderNumber> Create(Guid userId)
    {
        return new OrderNumber(GenerateOrderNumber(userId));
    }

    private static string GenerateOrderNumber(Guid userId)
    {
        return
            $"ORDER-{DateTime.UtcNow:yyyyMMdd}-{userId.ToString("N")[..8].ToUpper()}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    // Method for EF conversion
    public static OrderNumber FromValue(string value)
    {
        return new OrderNumber(value);
    }
}
