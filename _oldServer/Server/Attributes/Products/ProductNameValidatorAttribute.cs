using System.ComponentModel.DataAnnotations;

using Server.Entities;

namespace Server.Attributes.Products
{
    public class ProductNameValidatorAttribute : ValidationAttribute
    {
        public ProductNameValidatorAttribute()
            : base("Product with name {0} does not exist.") { }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;
            var productName = value as string;

            if (!string.IsNullOrWhiteSpace(productName))
            {
                var productExists = dbContext.Products.Any(
                    product => product.ProductName == productName
                );
                if (!productExists)
                {
                    return new ValidationResult(FormatErrorMessage(productName));
                }
            }

            return ValidationResult.Success;
        }
    }
}
