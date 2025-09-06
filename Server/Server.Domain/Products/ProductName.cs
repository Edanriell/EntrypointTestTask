using Server.Domain.Abstractions;

namespace Server.Domain.Products;

public sealed record ProductName
{
    private const int MaxNameLength = 100;

    public static readonly Error InvalidName = new(
        "ProductName.InvalidName",
        "Product name cannot be null or empty"
    );

    public static readonly Error NameTooLong = new(
        "ProductName.NameTooLong",
        "Product name cannot exceed 100 characters"
    );

    private ProductName(string value) { Value = value; }
    public string Value { get; init; }

    public static Result<ProductName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<ProductName>(InvalidName);
        }

        if (value.Length > MaxNameLength)
        {
            return Result.Failure<ProductName>(NameTooLong);
        }

        return new ProductName(value);
    }
}
 
 
