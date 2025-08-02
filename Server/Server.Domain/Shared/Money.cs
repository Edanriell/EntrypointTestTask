using Server.Domain.Abstractions;

namespace Server.Domain.Shared;

public sealed record Money(decimal Amount, Currency Currency)
{
    public static readonly Error InvalidPrice = new(
        "Money.InvalidPrice",
        "Product price must be greater than zero"
    );

    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to be equal");
        }

        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money operator -(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to be equal");
        }

        return new Money(first.Amount - second.Amount, first.Currency);
    }

    public static Money operator *(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to be equal");
        }

        return new Money(first.Amount * second.Amount, first.Currency);
    }

    public static Money operator *(Money money, Quantity quantity) =>
        new(money.Amount * quantity.Value, money.Currency);

    public static Money operator *(Quantity quantity, Money money) =>
        new(money.Amount * quantity.Value, money.Currency);

    public static bool operator <(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            return false;
        }

        return first.Amount < second.Amount;
    }

    public static bool operator >(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            return false;
        }

        return first.Amount > second.Amount;
    }

    public static bool operator <=(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            return false;
        }

        return first.Amount <= second.Amount;
    }

    public static bool operator >=(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            return false;
        }

        return first.Amount >= second.Amount;
    }

    public static Money Zero() { return new Money(0, Currency.None); }

    public static Money Zero(Currency currency) { return new Money(0, currency); }

    public bool IsZero() { return this == Zero(Currency); }

    public static Result<Money> Create(decimal amount, Currency currency)
    {
        if (amount <= 0)
        {
            return Result.Failure<Money>(InvalidPrice);
        }

        return new Money(amount, currency);
    }

    public static Result<Money> CreatePrice(Money price)
    {
        if (price is not { Amount: > 0 })
        {
            return Result.Failure<Money>(InvalidPrice);
        }

        return new Money(price.Amount, price.Currency);
    }
}
 
