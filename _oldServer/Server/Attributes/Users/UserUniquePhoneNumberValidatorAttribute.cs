using System.ComponentModel.DataAnnotations;
using Server.Entities;
using System.Text.RegularExpressions;

namespace Server.Attributes.Users
{
    public class UserUniquePhoneNumberValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;

            if (string.IsNullOrWhiteSpace(value!.ToString()))
                return new ValidationResult("Phone number cannot be empty or contain only spaces.");

            string phoneNumber = value.ToString()!;

            if (!IsPhoneNumberValid(phoneNumber))
                return new ValidationResult($"Provided phone number '{phoneNumber}' is not valid.");

            var isPhoneNumberAlreadyTaken = dbContext.Users.Any(
                user => user.PhoneNumber!.ToLower() == phoneNumber.ToLower()
            );

            if (isPhoneNumberAlreadyTaken)
                return new ValidationResult(
                    $"Phone number '{phoneNumber}' is already taken by another user."
                );

            return ValidationResult.Success;
        }

        private bool IsPhoneNumberValid(string phoneNumber)
        {
            string pattern = @"^\+(?:[0-9] ?){6,14}[0-9]$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(phoneNumber);
        }
    }
}
