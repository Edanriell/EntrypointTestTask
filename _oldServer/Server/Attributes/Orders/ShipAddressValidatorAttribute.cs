using Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace Server.Attributes.Orders
{
    public class ShipAddressValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;

            if (!(value is string address) || address.Length == 0 || address.Length >= 80)
                return new ValidationResult(
                    $"Ship address must contain at least one character and no more than 80 characters."
                );

            var orders = dbContext.Orders.Where(order => order.ShipAddress.Contains(address));

            if (orders is null)
                return new ValidationResult(
                    $"Provided ship address '{address}' is partially or fully invalid."
                );

            return ValidationResult.Success;
        }
    }
}
