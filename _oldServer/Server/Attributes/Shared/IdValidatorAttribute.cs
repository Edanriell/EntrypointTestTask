using System.ComponentModel.DataAnnotations;

using Server.Entities;
using Server.Shared.Interfaces;

namespace Server.Attributes.Shared
{
    public class IdValidatorAttribute<T> : ValidationAttribute
        where T : class, IIdentifiable
    {
        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var dbContext = (ApplicationDbContext)
                validationContext.GetService(typeof(ApplicationDbContext))!;

            if (!(value is int id) || id <= 0)
                return new ValidationResult(
                    $"Id {value} is not valid. Id must be a positive integer."
                );

            var dbSet = dbContext.Set<T>();
            var entityExists = dbSet.Any(entity => entity.Id == id);

            if (!entityExists)
                return new ValidationResult(
                    $"Entity of type {typeof(T).Name} with id {id} does not exist."
                );

            return ValidationResult.Success;
        }
    }
}
