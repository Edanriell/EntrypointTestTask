using System.ComponentModel.DataAnnotations;

using Server.Entities;

namespace Server.Attributes.Products
{
    public class ProductCodeValidatorAttribute : ValidationAttribute
    {
        public ProductCodeValidatorAttribute()
            : base("Product code '{0}' fully or partially does not exist.") { }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;
            var productCode = value as string;

            if (!string.IsNullOrWhiteSpace(productCode))
            {
                var productCodeExists = dbContext.Products.Any(
                    product => product.Code.Contains(productCode)
                );
                if (!productCodeExists)
                {
                    return new ValidationResult(FormatErrorMessage(productCode));
                }
            }

            return ValidationResult.Success;
        }
    }
}
