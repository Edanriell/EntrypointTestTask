namespace Application.Products.Queries.GetProduct;

public class GetProductValidator
	: AbstractValidator<GetProductQuery>
{
	public GetProductValidator()
	{
		RuleFor(x => x.Id)
		   .NotEmpty()
		   .WithMessage("Product ID is required.")
		   .GreaterThan(0)
		   .WithMessage("Product ID must be greater than 0.");
	}
}