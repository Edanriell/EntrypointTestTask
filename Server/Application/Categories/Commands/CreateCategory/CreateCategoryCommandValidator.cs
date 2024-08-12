namespace Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
	public CreateCategoryCommandValidator()
	{
		RuleFor(x => x.Name)
		   .Length(6, 72)
		   .WithMessage("Category name must be between 6 and 72 characters long.");

		RuleFor(x => x.Description)
		   .Length(24, 500)
		   .WithMessage("Product description must be between 24 and 500 characters long.");
	}
}