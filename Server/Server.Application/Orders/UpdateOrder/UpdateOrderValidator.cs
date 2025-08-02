using FluentValidation;

namespace Server.Application.Orders.UpdateOrder;

public sealed class UpdateOrderValidator : AbstractValidator<UpdateOrder>
{
    public UpdateOrderValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.Street)
            .MaximumLength(200)
            .WithMessage("Street address cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Street));

        RuleFor(x => x.City)
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.ZipCode)
            .Matches(@"^\d{5}(-\d{4})?$")
            .WithMessage("Zip code must be in format 12345 or 12345-6789")
            .When(x => !string.IsNullOrWhiteSpace(x.ZipCode));

        RuleFor(x => x.Country)
            .MaximumLength(100)
            .WithMessage("Country cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Country));

        RuleFor(x => x.Info)
            .MaximumLength(500)
            .WithMessage("Info cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Info));
    }
}
