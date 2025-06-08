using System.ComponentModel.DataAnnotations;

using Server.Entities;

namespace Server.Attributes.Users
{
    public class UserNameValidatorAttribute : ValidationAttribute
    {
        public UserNameValidatorAttribute()
            : base("Customer with name {0} does not exist.") { }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;
            var userName = value as string;

            if (!string.IsNullOrWhiteSpace(userName))
            {
                var customerExists = dbContext.Users.Any(user => user.Name == userName);
                if (!customerExists)
                {
                    return new ValidationResult(FormatErrorMessage(userName));
                }
            }

            return ValidationResult.Success;
        }
    }
}
