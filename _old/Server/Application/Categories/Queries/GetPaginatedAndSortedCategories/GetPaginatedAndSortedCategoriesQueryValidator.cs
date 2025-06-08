using Domain.Entities;

namespace Application.Categories.Queries.GetPaginatedAndSortedCategories;

public class GetPaginatedAndSortedCategoriesQueryValidator : AbstractValidator<GetPaginatedAndSortedCategoriesQuery>
{
	public GetPaginatedAndSortedCategoriesQueryValidator()
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
	}

	private bool BeValidSortColumn(string sortColumn)
	{
		var orderEntityProperties = typeof(Category).GetProperties();

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