namespace Application.Orders.Queries.GetOrder;

public class GetOrderValidator
	: AbstractValidator<GetOrderQuery>
{
	public GetOrderValidator()
	{
		RuleFor(x => x.Id)
		   .NotEmpty()
		   .WithMessage("Order ID is required.")
		   .GreaterThan(0)
		   .WithMessage("Order ID must be greater than 0.");
	}
}