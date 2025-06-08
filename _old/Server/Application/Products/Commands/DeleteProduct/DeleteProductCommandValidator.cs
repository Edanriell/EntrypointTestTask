namespace Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
	public DeleteProductCommandValidator()
	{
		RuleFor(x => x.Id)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Order id must be greater than 0");

		RuleFor(x => x.ProductName)
		   .NotEmpty()
		   .WithMessage("Product name can't be empty")
		   .Length(6, 64)
		   .WithMessage("Product name must be at least 6 chars and no more than 64 chars long");
	}
}