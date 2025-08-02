using Server.Domain.Abstractions;

namespace Server.Domain.Products;

public sealed record ProductDescription
{
    private const int MaxDescriptionLength = 1000;

    public static readonly Error InvalidDescription = new(
        "ProductDescription.InvalidDescription",
        "Product description cannot be null or empty"
    );

    public static readonly Error DescriptionTooLong = new(
        "ProductDescription.DescriptionTooLong",
        "Product description cannot exceed 1000 characters"
    );

    private ProductDescription(string value) { Value = value; }

    public string Value { get; init; }

    public static Result<ProductDescription> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<ProductDescription>(InvalidDescription);
        }

        if (value.Length > MaxDescriptionLength)
        {
            return Result.Failure<ProductDescription>(DescriptionTooLong);
        }

        return new ProductDescription(value);
    }
}
 
