using System.ComponentModel.DataAnnotations;

using Server.Entities;

namespace Server.Attributes.Users
{
    public class UserUsernameValidatorAttribute : ValidationAttribute
    {
        public UserUsernameValidatorAttribute()
            : base("Customer with username {0} does not exist.") { }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;
            var username = value as string;

            //if (!string.IsNullOrWhiteSpace(username))
            //{
            //    var usernameExists = dbContext.Users.Any(user => user.Username == username);
            //    if (!usernameExists)
            //    {
            //        return new ValidationResult(FormatErrorMessage(username));
            //    }
            //}

            return ValidationResult.Success;
        }
    }
}
