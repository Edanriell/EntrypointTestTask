namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
	public UpdateProductCommandValidator()
	{
		RuleFor(x => x.Id)
		   .GreaterThan(0)
		   .WithMessage("The product ID must be greater than 0.");

		RuleFor(x => x.Code)
		   .MinimumLength(4)
		   .WithMessage("Product code must be at least 4 characters long.")
		   .MaximumLength(50)
		   .WithMessage("Product code must not exceed 50 characters.")
		   .When(x => !string.IsNullOrWhiteSpace(x.Code));

		RuleFor(x => x.ProductName)
		   .Length(6, 100)
		   .WithMessage("Product name must be between 6 and 100 characters long.")
		   .When(x => !string.IsNullOrWhiteSpace(x.ProductName));

		RuleFor(x => x.Description)
		   .Length(24, 324)
		   .WithMessage("Product description must be between 24 and 324 characters long.")
		   .When(x => !string.IsNullOrWhiteSpace(x.Description));

		RuleFor(x => x.UnitPrice)
		   .GreaterThan(0)
		   .WithMessage("Unit price must be a positive value.")
		   .When(x => x.UnitPrice.HasValue);
		;

		RuleFor(x => x.UnitsInStock)
		   .GreaterThan(0)
		   .WithMessage("Units in stock must be greater than 0.")
		   .When(x => x.UnitsInStock.HasValue);

		RuleFor(x => x.UnitsOnOrder)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Units on order must be a non-negative integer.")
		   .When(x => x.UnitsOnOrder.HasValue);

		RuleFor(x => x.Brand)
		   .NotEmpty()
		   .WithMessage("Brand name must be provided.")
		   .MaximumLength(80)
		   .WithMessage("Brand name must not exceed 80 characters.")
		   .When(x => !string.IsNullOrWhiteSpace(x.Brand));

		RuleFor(x => x.Rating)
		   .InclusiveBetween(0, 10)
		   .WithMessage("Product rating must be between 0 and 10.")
		   .When(x => x.Rating.HasValue);

		RuleFor(x => x.Photo)
		   .Must(photo => IsValidBase64String(photo))
		   .WithMessage("The provided photo must be a valid base64 string.")
		   .When(x => !string.IsNullOrWhiteSpace(x.Photo));

		RuleForEach(x => x.UpdatedCategories)
		   .ChildRules(categories =>
			{
				categories.RuleFor(category => category.Id)
				   .GreaterThan(0)
				   .WithMessage("Each category ID must be a positive integer.");
			})
		   .When(x => x.UpdatedCategories is not null)
		   .WithMessage("Categories must be provided when specified.");

		RuleFor(x => x.UpdatedCategories)
		   .NotEmpty()
		   .When(x => x.UpdatedCategories is not null)
		   .WithMessage("At least one category must be provided when the categories list is specified.");
	}

	// TODO
	// Test photo validation
	// Probably will not work as expected
	private bool IsValidBase64String(string base64String)
	{
		var buffer = new Span<byte>(new byte[base64String.Length]);
		return Convert.TryFromBase64String(base64String, buffer, out _);
	}
}