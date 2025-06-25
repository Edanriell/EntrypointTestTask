using FluentValidation;

namespace Server.Application.Orders.UpdateOrderShippingAddress;

public sealed class UpdateOrderShippingAddressCommandValidator : AbstractValidator<UpdateOrderShippingAddressCommand>
{
    public UpdateOrderShippingAddressCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street address is required")
            .MaximumLength(200)
            .WithMessage("Street address cannot exceed 200 characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("Zip code is required")
            .Matches(@"^\d{5}(-\d{4})?$")
            .WithMessage("Zip code must be in format 12345 or 12345-6789");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .MaximumLength(100)
            .WithMessage("Country cannot exceed 100 characters");
    }
}
