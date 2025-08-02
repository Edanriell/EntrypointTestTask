﻿using Server.Domain.Abstractions;

namespace Server.Domain.Shared;

public sealed record Currency
{
    internal static readonly Currency None = new("");
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");
    public static readonly IReadOnlyCollection<Currency> All = new[] { Usd, Eur };

    private Currency(string code) { Code = code; }
    public string Code { get; init; }

    public static Result<Currency> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result.Failure<Currency>(CurrencyErrors.EmptyCurrencyCode);
        }

        Currency? currency = All.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

        return currency is not null
            ? Result.Success(currency)
            : Result.Failure<Currency>(CurrencyErrors.InvalidCurrencyCode);
    }

    public static Currency FromCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return None;
        }

        return All.FirstOrDefault(c => c.Code == code) ??
            throw new ApplicationException("The currency code is invalid");
    }
}
