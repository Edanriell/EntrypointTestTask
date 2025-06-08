namespace Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
	public UpdateCategoryCommandValidator()
	{
		RuleFor(x => x.Id)
		   .GreaterThan(0)
		   .WithMessage("The product ID must be greater than 0.");

		RuleFor(x => x.CategoryName)
		   .Length(6, 72)
		   .WithMessage("Category name must be between 6 and 100 characters long.")
		   .When(x => !string.IsNullOrWhiteSpace(x.CategoryName));

		RuleFor(x => x.CategoryDescription)
		   .Length(24, 500)
		   .WithMessage("Category description must be between 24 and 324 characters long.")
		   .When(x => !string.IsNullOrWhiteSpace(x.CategoryDescription));
	}
}