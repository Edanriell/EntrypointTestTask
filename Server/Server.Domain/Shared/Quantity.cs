using Server.Domain.Abstractions;

namespace Server.Domain.Shared;

public sealed record Quantity
{
    public static readonly Error InvalidQuantity = new(
        "Quantity.InvalidQuantity",
        "Product quantity cannot be negative"
    );

    private Quantity(int value) { Value = value; }

    public int Value { get; init; }
 
    public static Quantity operator +(Quantity first, Quantity second) => new(first.Value + second.Value);
    public static Quantity operator -(Quantity first, Quantity second) => new(first.Value - second.Value);

    public static Result<Quantity> CreateQuantity(int value)
    {
        if (value < 0)
        {
            return Result.Failure<Quantity>(InvalidQuantity);
        }

        return new Quantity(value);
    }

    public static Quantity CreateStock(int value)
    {
        return new Quantity(value);
    }
} 
