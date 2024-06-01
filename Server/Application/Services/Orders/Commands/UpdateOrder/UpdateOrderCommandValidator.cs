namespace Application.Services.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Order id must be greater than 0");

        RuleFor(x => x.OrderStatus)
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.OrderInformation)
            .Length(6, 128)
            .WithMessage("Order information must be at least 6 characters and no more than 36 chars long");

        RuleFor(x => x.ShipAddress)
            .Length(6, 128)
            .WithMessage("Ship address must be at least 6 characters and no more than 128 characters long");
    }
}