using System.ComponentModel.DataAnnotations;

using Server.Entities;

namespace Server.Attributes.Users
{
    public class UserEmailValidatorAttribute : ValidationAttribute
    {
        public UserEmailValidatorAttribute()
            : base("Customer with email {0} does not exist.") { }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;
            var userEmail = value as string;

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                var customerExists = dbContext.Users.Any(user => user.Email == userEmail);
                if (!customerExists)
                {
                    return new ValidationResult(FormatErrorMessage(userEmail));
                }
            }

            return ValidationResult.Success;
        }
    }
}
