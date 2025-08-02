using FluentValidation;

namespace Server.Application.Orders.CreateOrder;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage("Client ID is required");

        RuleFor(x => x.OrderNumber)
            .NotEmpty()
            .WithMessage("Order number is required")
            .MaximumLength(50)
            .WithMessage("Order number cannot exceed 50 characters");

        RuleFor(x => x.ShippingAddress)
            .NotNull()
            .WithMessage("Shipping address is required")
            .SetValidator(new ShippingAddressValidator());

        RuleFor(x => x.OrderItems)
            .NotNull()
            .WithMessage("Order items cannot be null")
            .NotEmpty()
            .WithMessage("Order must contain at least one item")
            .Must(HaveUniqueProducts)
            .WithMessage("Order cannot contain duplicate products");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemValidator());
    }

    private static bool HaveUniqueProducts(List<OrderItem> orderItems)
    {
        if (orderItems == null)
        {
            return true;
        }

        var productIds = orderItems.Select(x => x.ProductId).ToList();
        return productIds.Count == productIds.Distinct().Count();
    }
}

public sealed class ShippingAddressValidator : AbstractValidator<ShippingAddress>
{
    public ShippingAddressValidator()
    {
        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .MaximumLength(100)
            .WithMessage("Country cannot exceed 100 characters")
            .Matches("^[a-zA-Z\\s-']+$")
            .WithMessage("Country contains invalid characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters")
            .Matches("^[a-zA-Z\\s-']+$")
            .WithMessage("City contains invalid characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("Zip code is required")
            .MaximumLength(20)
            .WithMessage("Zip code cannot exceed 20 characters")
            .Matches("^[a-zA-Z0-9\\s-]+$")
            .WithMessage("Zip code contains invalid characters");

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required")
            .MaximumLength(200)
            .WithMessage("Street cannot exceed 200 characters")
            .MinimumLength(5)
            .WithMessage("Street must be at least 5 characters long");
    }
}

public sealed class OrderItemValidator : AbstractValidator<OrderItem>
{
    public OrderItemValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Quantity cannot exceed 1000 per item");
    }
}
 
