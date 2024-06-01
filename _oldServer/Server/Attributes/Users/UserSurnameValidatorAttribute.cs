using System.ComponentModel.DataAnnotations;

using Server.Entities;

namespace Server.Attributes.Users;

public class UserSurnameValidatorAttribute : ValidationAttribute
{
    public UserSurnameValidatorAttribute()
        : base("Customer with surname {0} does not exist.") { }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var dbContext = (ApplicationDbContext)
            validationContext.GetService(typeof(ApplicationDbContext))!;
        var userSurname = value as string;

        if (!string.IsNullOrWhiteSpace(userSurname))
        {
            var customerExists = dbContext.Users.Any(user => user.Surname == userSurname);
            if (!customerExists)
            {
                return new ValidationResult(FormatErrorMessage(userSurname));
            }
        }

        return ValidationResult.Success;
    }
}
