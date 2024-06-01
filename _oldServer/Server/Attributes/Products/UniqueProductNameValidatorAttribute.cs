using Microsoft.IdentityModel.Tokens;
using Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace Server.Attributes.Products
{
    public class UniqueProductNameValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;

            if (string.IsNullOrWhiteSpace(value!.ToString()))
                return new ValidationResult("Product name could not be empty.");

            var productName = value.ToString();

            if (productName!.Length > 400)
                return new ValidationResult("Product name can't contain more than 100 characters.");

            var productExists = dbContext.Products.Any(
                product => product.ProductName == productName
            );

            if (productExists)
            {
                return new ValidationResult($"Product with name {productName} already exists.");
            }

            return ValidationResult.Success;
        }
    }
}
