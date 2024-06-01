namespace Application.Services.Products.Queries.GetProduct;

public class GetProductValidator
    : AbstractValidator<GetProductQuery>
{
    public GetProductValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ProductId must be greater than 0.");
    }
}