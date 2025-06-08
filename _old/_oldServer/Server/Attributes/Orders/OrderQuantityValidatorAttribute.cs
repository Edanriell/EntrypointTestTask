using System.ComponentModel.DataAnnotations;

using Server.Entities;
using Server.DTO.Products;

namespace Server.Attributes.Orders
{
    public class OrderQuantityValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;

            var productIdsWithQuantities = (ProductIdsWithQuantitiesDTO)
                validationContext.ObjectInstance;

            if (!(value is short quantity) || quantity <= 0)
            {
                return new ValidationResult("Quantity must be a positive integer.");
            }

            var product = dbContext.Products
                .Where(product => product.Id == productIdsWithQuantities.ProductId)
                .FirstOrDefault();

            if (product is null)
                return new ValidationResult(
                    $"Product with Id {productIdsWithQuantities.ProductId} does not exist."
                );

            var unitsInStock = product.UnitsInStock;

            var unitsOnOrder = product.UnitsOnOrder;

            if (unitsInStock <= 0)
                return new ValidationResult($"Product {product.ProductName} is out of stock.");

            var availableUnits = unitsInStock - unitsOnOrder;

            if ((availableUnits - quantity) < 0)
                return new ValidationResult(
                    $"Only {quantity} units left for product {product.ProductName}"
                );

            return ValidationResult.Success;
        }
    }
}
