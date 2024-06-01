namespace Application.Services.Orders.Queries.GetOrder;

public class GetOrderValidator
    : AbstractValidator<GetOrderQuery>
{
    public GetOrderValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("Please provide a valid order id")
            .GreaterThan(0)
            .WithMessage("Order id must be greater than 0");
    }
}