using Domain.Entities;

namespace Application.Users.Queries.GetPaginatedSortedAndFilteredUsers;

public class GetPaginatedSortedAndFilteredUsersValidator
	: AbstractValidator<GetPaginatedSortedAndFilteredUsersQuery>
{
	public GetPaginatedSortedAndFilteredUsersValidator()
	{
		RuleFor(x => x.PageIndex)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Page index must be greater than or equal to 0.")
		   .LessThanOrEqualTo(100)
		   .WithMessage("Page index must not exceed 100.");

		RuleFor(x => x.PageSize)
		   .GreaterThanOrEqualTo(1)
		   .WithMessage("Page size must be greater than or equal to 1.")
		   .LessThanOrEqualTo(100)
		   .WithMessage("Page size must not exceed 100.");

		RuleFor(x => x.SortColumn)
		   .Length(2, 24)
		   .WithMessage("Sort column name must be between 2 and 24 characters long.")
		   .Must(BeValidSortColumn)
		   .WithMessage("Specified sort column is not valid for the product entity.");

		RuleFor(x => x.SortOrder)
		   .Length(3, 4)
		   .WithMessage("Sort order must be either 'ASC' or 'DESC'.")
		   .Must(BeValidSortOrder)
		   .WithMessage("Sort order must be either 'ASC' for ascending or 'DESC' for descending order.");

		RuleFor(x => x.UserName)
		   .Length(3, 50)
		   .WithMessage("Username must be between 3 and 50 characters.");

		RuleFor(x => x.Name)
		   .Length(2, 50)
		   .WithMessage("Name must be between 2 and 50 characters.");

		RuleFor(x => x.Surname)
		   .Length(2, 50)
		   .WithMessage("Surname must be between 2 and 50 characters.");

		RuleFor(x => x.Email)
		   .EmailAddress()
		   .WithMessage("A valid email is required.");

		RuleFor(x => x.BirthDate)
		   .LessThanOrEqualTo(DateTime.Now)
		   .WithMessage("Birth date cannot be in the future.");

		RuleFor(x => x.CreatedAt)
		   .LessThanOrEqualTo(DateTime.Now)
		   .WithMessage("Created date cannot be in the future.");

		RuleFor(x => x.LastLogin)
		   .LessThanOrEqualTo(DateTime.Now)
		   .WithMessage("Last login date cannot be in the future.");

		RuleFor(x => x.Gender)
		   .IsInEnum()
		   .WithMessage("A valid gender is required.");

		RuleFor(x => x.Address)
		   .MaximumLength(140)
		   .WithMessage("Address can't be longer than 100 characters.");

		RuleFor(x => x.Role)
		   .Length(2, 50)
		   .WithMessage("Role must be between 2 and 50 characters.");

		RuleFor(x => x.RegistrationStart)
		   .LessThanOrEqualTo(x => x.RegistrationEnd)
		   .When(x => x.RegistrationStart.HasValue && x.RegistrationEnd.HasValue)
		   .WithMessage("Registration start date must be earlier than or equal to the end date.");

		RuleFor(x => x.RegistrationEnd)
		   .GreaterThanOrEqualTo(x => x.RegistrationStart)
		   .When(x => x.RegistrationEnd.HasValue && x.RegistrationStart.HasValue)
		   .WithMessage("Registration end date must be later than or equal to the start date.");

		RuleFor(x => x.AgeFrom)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Minimum age cannot be negative.")
		   .LessThanOrEqualTo(x => x.AgeTo ?? int.MaxValue)
		   .WithMessage("Minimum age cannot be greater than maximum age.");

		RuleFor(x => x.AgeTo)
		   .GreaterThanOrEqualTo(x => x.AgeFrom ?? 0)
		   .WithMessage("Maximum age cannot be less than minimum age.");
	}

	private bool BeValidSortColumn(string sortColumn)
	{
		var orderEntityProperties = typeof(Order).GetProperties();

		return orderEntityProperties.Any(prop => prop.Name == sortColumn);
	}

	private bool BeValidSortOrder(string sortOrder)
	{
		return sortOrder switch
			   {
				   "ASC" => true, "DESC" => true, _ => false
			   };
	}
}