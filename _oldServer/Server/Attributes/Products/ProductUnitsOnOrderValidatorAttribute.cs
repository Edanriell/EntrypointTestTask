using System.ComponentModel.DataAnnotations;

namespace Server.Attributes.Products
{
    public class ProductUnitsOnOrderValidatorAttribute : ValidationAttribute
    {
        bool _allowZero;

        public ProductUnitsOnOrderValidatorAttribute(bool allowZero = false)
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
                        "The value of ordered units must not be a negative integer or equal to zero."
                    );

            if (_allowZero)
                if (value is null || value is short shortValue && shortValue < 0)
                    return new ValidationResult(
                        "The value of ordered units must not be a negative integer."
                    );

            return ValidationResult.Success;
        }
    }
}
