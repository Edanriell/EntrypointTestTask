namespace Application.Orders.Commands.DeleteOrder;

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
	public DeleteOrderCommandValidator()
	{
		RuleFor(x => x.Id)
		   .GreaterThan(0)
		   .WithMessage("Order ID must be greater than 0.");
	}
}