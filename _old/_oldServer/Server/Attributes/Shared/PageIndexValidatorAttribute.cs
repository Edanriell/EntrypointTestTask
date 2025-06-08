using System.ComponentModel.DataAnnotations;

namespace Server.Attributes.Shared
{
    public class PageIndexValidatorAttribute : ValidationAttribute
    {
        public PageIndexValidatorAttribute()
            : base("The value must not be a negative integer.") { }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (value is int intValue && intValue >= 0)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage);
        }
    }
}
