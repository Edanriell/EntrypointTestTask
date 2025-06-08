namespace Application.Categories.Commands.UpdateCategoryProducts;

public class UpdateCategoryProductsCommandValidator : AbstractValidator<UpdateCategoryProductsCommand>
{
	public UpdateCategoryProductsCommandValidator()
	{
		RuleFor(x => x.Id)
		   .GreaterThan(0)
		   .WithMessage("Category ID must be a positive integer.");

		RuleFor(x => x.UpdatedCategoryProducts)
		   .NotNull()
		   .WithMessage("The list of updated category products cannot be null.")
		   .NotEmpty()
		   .WithMessage("The list of updated category products cannot be empty.");

		RuleForEach(x => x.UpdatedCategoryProducts)
		   .ChildRules(products =>
			{
				products.RuleFor(product => product.Id)
				   .GreaterThan(0)
				   .WithMessage("Each product ID must be a positive integer.");
			});
	}
}