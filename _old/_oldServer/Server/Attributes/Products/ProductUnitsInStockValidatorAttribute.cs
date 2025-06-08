using System.ComponentModel.DataAnnotations;

namespace Server.Attributes.Products
{
    public class ProductUnitsInStockValidatorAttribute : ValidationAttribute
    {
        bool _allowZero;

        public ProductUnitsInStockValidatorAttribute(bool allowZero = false)
        {
            _allowZero = allowZero;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (!_allowZero)
                if (value is null || value is short shortValue && shortValue <= 0)
                    return new ValidationResult(
                        "The value of units in stock must not be a negative integer or equal to zero."
                    );

            if (_allowZero)
                if (value is null || value is short shortValue && shortValue < 0)
                    return new ValidationResult(
                        "The value of units in stock must not be a negative integer."
                    );

            return ValidationResult.Success;
        }
    }
}
