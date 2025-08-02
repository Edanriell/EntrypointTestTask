using Server.Domain.Abstractions;

namespace Server.Domain.Shared;

public static class CurrencyErrors
{
    public static readonly Error EmptyCurrencyCode = new(
        "Currency.EmptyCode",
        "Currency code cannot be empty");

    public static readonly Error InvalidCurrencyCode = new(
        "Currency.InvalidCode",
        "The specified currency code is not supported");
}
