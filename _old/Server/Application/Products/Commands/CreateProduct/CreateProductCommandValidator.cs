namespace Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
	public CreateProductCommandValidator()
	{
		RuleFor(x => x.Code)
		   .MinimumLength(4)
		   .WithMessage("Product code must be at least 4 characters long.")
		   .MaximumLength(50)
		   .WithMessage("Product code must not exceed 50 characters.");

		RuleFor(x => x.Name)
		   .Length(6, 100)
		   .WithMessage("Product name must be between 6 and 100 characters long.");

		RuleFor(x => x.Description)
		   .Length(24, 324)
		   .WithMessage("Product description must be between 24 and 324 characters long.");

		RuleFor(x => x.UnitPrice)
		   .GreaterThan(0)
		   .WithMessage("Unit price must be a positive value.");

		RuleFor(x => x.UnitsInStock)
		   .GreaterThan(0)
		   .WithMessage("Units in stock must be greater than 0.");

		RuleFor(x => x.UnitsOnOrder)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Units on order must be a non-negative integer.");

		RuleFor(x => x.Brand)
		   .NotEmpty()
		   .WithMessage("Brand name must be provided.")
		   .MaximumLength(80)
		   .WithMessage("Brand name must not exceed 80 characters.");

		RuleFor(x => x.Rating)
		   .InclusiveBetween(0, 10)
		   .WithMessage("Product rating must be between 0 and 10.");

		RuleForEach(x => x.Categories)
		   .ChildRules(categories =>
			{
				categories.RuleFor(category => category.Id)
				   .GreaterThan(0)
				   .WithMessage("Each category ID must be a positive integer.");
			})
		   .When(x => x.Categories is not null)
		   .WithMessage("Categories must be provided when specified.");

		RuleFor(x => x.Categories)
		   .NotEmpty()
		   .When(x => x.Categories is not null)
		   .WithMessage("At least one category must be provided when the categories list is specified.");
	}
}