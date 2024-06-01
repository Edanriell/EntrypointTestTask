using System.ComponentModel.DataAnnotations;

namespace Server.Attributes.Products
{
    public class ProductUnitPriceValidatorAttribute : ValidationAttribute
    {
        public ProductUnitPriceValidatorAttribute()
            : base("The value of product unit price must not be a negative integer or equal to 0.")
        { }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value is decimal decimalValue && decimalValue > 0)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage);
        }
    }
}
