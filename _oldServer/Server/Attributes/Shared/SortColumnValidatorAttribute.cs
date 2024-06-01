using System.ComponentModel.DataAnnotations;

namespace Server.Attributes.Shared
{
    public class SortColumnValidatorAttribute : ValidationAttribute
    {
        public Type EntityType { get; set; }

        public SortColumnValidatorAttribute(Type entityType)
            : base("Value must match an existing column.")
        {
            EntityType = entityType;
        }

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            if (EntityType != null)
            {
                var strValue = value as string;
                if (
                    !string.IsNullOrWhiteSpace(strValue)
                    && EntityType.GetProperties().Any(property => property.Name == strValue)
                )
                    return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage, new[] { "SortColumn" });
        }
    }
}
