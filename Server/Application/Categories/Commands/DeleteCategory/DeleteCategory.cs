namespace Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
	public DeleteCategoryCommandValidator()
	{
		RuleFor(x => x.Id)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Category id must be greater than 0");

		RuleFor(x => x.CategoryName)
		   .NotEmpty()
		   .WithMessage("Category name can't be empty")
		   .Length(6, 72)
		   .WithMessage("Category name must be at least 6 chars and no more than 72 chars long");
	}
}