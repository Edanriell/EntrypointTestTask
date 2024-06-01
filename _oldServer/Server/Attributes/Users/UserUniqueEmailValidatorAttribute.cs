using System.ComponentModel.DataAnnotations;
using Server.Entities;
using System.Text.RegularExpressions;

namespace Server.Attributes.Users
{
    public class UserUniqueEmailValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;

            if (string.IsNullOrWhiteSpace(value!.ToString()))
                return new ValidationResult("User email cannot be empty or contain only spaces.");

            string email = value.ToString()!;

            if (!IsEmailValid(email))
                return new ValidationResult($"Provided email '{email}' is not valid.");

            var isEmailAlreadyTaken = dbContext.Users.Any(
                user => user.Email!.ToLower() == email.ToLower()
            );

            if (isEmailAlreadyTaken)
                return new ValidationResult($"Email '{email}' is already taken by another user.");

            return ValidationResult.Success;
        }

        private bool IsEmailValid(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
    }
}
