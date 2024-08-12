using Domain.Entities;

namespace Application.Products.Queries.GetPaginatedSortedAndFilteredProducts;

public class GetProductsWithPaginationSortingAndFilteringValidator
	: AbstractValidator<GetPaginatedSortedAndFilteredProductsQuery>
{
	public GetProductsWithPaginationSortingAndFilteringValidator()
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

		RuleFor(x => x.Code)
		   .MinimumLength(4)
		   .WithMessage("Product code must be at least 4 characters long.")
		   .MaximumLength(50)
		   .WithMessage("Product code must not exceed 50 characters.");

		RuleFor(x => x.ProductName)
		   .MinimumLength(4)
		   .WithMessage("Product name must be at least 4 characters long.")
		   .MaximumLength(100)
		   .WithMessage("Product name must not exceed 100 characters.");

		RuleFor(x => x.UnitsInStock)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Units in stock must be a non-negative value.");

		RuleFor(x => x.UnitsOnOrder)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Units on order must be a non-negative value.");

		RuleFor(x => x.CustomerName)
		   .MinimumLength(4)
		   .WithMessage("Customer name must be at least 4 characters long.")
		   .MaximumLength(50)
		   .WithMessage("Customer name must not exceed 50 characters.");

		RuleFor(x => x.CustomerSurname)
		   .MinimumLength(4)
		   .WithMessage("Customer surname must be at least 4 characters long.")
		   .MaximumLength(50)
		   .WithMessage("Customer surname must not exceed 50 characters.");

		RuleFor(x => x.CustomerEmail)
		   .EmailAddress()
		   .WithMessage("Customer email address must be a valid email format.");

		RuleFor(x => x.Category)
		   .MinimumLength(2)
		   .WithMessage("Category name must be at least 2 characters long.")
		   .MaximumLength(72)
		   .WithMessage("Category name must not exceed 72 characters.");

		RuleFor(x => x.MinPrice)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Minimum price must be a non-negative value.");

		RuleFor(x => x.MaxPrice)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Maximum price must be a non-negative value.")
		   .GreaterThanOrEqualTo(x => x.MinPrice)
		   .WithMessage("Maximum price must be greater than or equal to the minimum price.");

		RuleFor(x => x.MinRating)
		   .InclusiveBetween(0, 10)
		   .WithMessage("Minimum rating must be between 0 and 10.");

		RuleFor(x => x.MaxRating)
		   .InclusiveBetween(0, 10)
		   .WithMessage("Maximum rating must be between 0 and 10.")
		   .GreaterThanOrEqualTo(x => x.MinRating)
		   .WithMessage("Maximum rating must be greater than or equal to the minimum rating.");
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