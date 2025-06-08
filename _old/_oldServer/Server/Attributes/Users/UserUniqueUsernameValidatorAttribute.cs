using System.ComponentModel.DataAnnotations;
using Server.Entities;
using System.Text.RegularExpressions;

namespace Server.Attributes.Users
{
    public class UserUniqueUsernameValidatorAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;

            if (string.IsNullOrWhiteSpace(value!.ToString()))
                return new ValidationResult("Username cannot be empty or contain only spaces.");

            string username = value.ToString()!;

            //var isUsernameAlreadyTaken = dbContext.Users.Any(
            //    user => user.Username.ToLower() == username.ToLower()
            //);

            //if (isUsernameAlreadyTaken)
            //    return new ValidationResult($"Username '{username}' is already taken.");

            return ValidationResult.Success;
        }
    }
}
