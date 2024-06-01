using System.ComponentModel.DataAnnotations;

namespace Server.Attributes.Shared
{
    public class SortOrderValidatorAttribute : ValidationAttribute
    {
        public string[] AllowedValues { get; set; } = new[] { "ASC", "DESC" };

        public SortOrderValidatorAttribute()
            : base("Value must be one of the following: {0}.") { }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var strValue = value as string;
            if (!string.IsNullOrWhiteSpace(strValue) && AllowedValues.Contains(strValue))
                return ValidationResult.Success;
            return new ValidationResult(FormatErrorMessage(string.Join(",", AllowedValues)));
        }
    }
}
