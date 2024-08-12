using Application.Orders.Dto;

namespace Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
	public UpdateOrderCommandValidator()
	{
		RuleFor(x => x.Id)
		   .GreaterThan(0)
		   .WithMessage("Order ID must be greater than 0.");

		RuleFor(x => x.OrderStatus)
		   .IsInEnum()
		   .WithMessage("Invalid order status.")
		   .When(x => x.OrderStatus.HasValue);

		RuleFor(x => x.AdditionalInformation)
		   .MaximumLength(400)
		   .WithMessage("Additional information cannot exceed 400 characters.")
		   .When(x => !string.IsNullOrEmpty(x.AdditionalInformation));

		RuleFor(x => x.ShippingName)
		   .Length(2, 100)
		   .WithMessage("Shipping name must be between 2 and 100 characters long.")
		   .When(x => !string.IsNullOrEmpty(x.ShippingName));

		RuleFor(x => x.ShippingAddress)
		   .SetValidator(new AddressValidator()!)
		   .When(x => x.ShippingAddress is not null);

		RuleFor(x => x.BillingAddress)
		   .SetValidator(new AddressValidator()!)
		   .When(x => x.BillingAddress is not null);

		RuleFor(x => x.UpdatedProducts)
		   .ForEach(product => product.SetValidator(new ProductIdsWithQuantitiesDtoValidator()))
		   .When(x => x.UpdatedProducts is not null && x.UpdatedProducts.Any());

		RuleFor(x => x.UpdatedPayments)
		   .ForEach(payment => payment.SetValidator(new PaymentValidator()))
		   .When(x => x.UpdatedPayments is not null && x.UpdatedPayments.Any());
	}
}

public class AddressValidator : AbstractValidator<AddressDto>
{
	public AddressValidator()
	{
		RuleFor(x => x.Street)
		   .NotEmpty()
		   .WithMessage("Street address cannot be empty.")
		   .Length(5, 50)
		   .WithMessage("Street address must be between 5 and 50 characters long.");

		RuleFor(x => x.City)
		   .NotEmpty()
		   .WithMessage("City cannot be empty.")
		   .Length(2, 50)
		   .WithMessage("City must be between 2 and 50 characters long.");

		RuleFor(x => x.Region)
		   .MaximumLength(50)
		   .WithMessage("Region must be no more than 50 characters long.")
		   .When(x => !string.IsNullOrEmpty(x.Region));

		RuleFor(x => x.PostalCode)
//		   .Matches(@"^\d{5}(?:[-\s]\d{4})?$")
//		   .WithMessage("Invalid postal code format.")
		   .Length(2, 18)
		   .WithMessage("Postal code must be between 2 and 18 characters long.")
		   .When(x => !string.IsNullOrEmpty(x.PostalCode));

		RuleFor(x => x.Country)
		   .NotEmpty()
		   .WithMessage("Country cannot be empty.")
		   .Length(2, 50)
		   .WithMessage("Country must be between 2 and 50 characters long.");
	}
}

public class ProductIdsWithQuantitiesDtoValidator : AbstractValidator<ProductIdWithQuantityDto>
{
	public ProductIdsWithQuantitiesDtoValidator()
	{
		RuleFor(x => x.ProductId)
		   .GreaterThan(0)
		   .WithMessage("Product ID must be greater than 0.");

		RuleFor(x => x.Quantity)
		   .GreaterThan(0)
		   .WithMessage("Quantity must be greater than 0.");
	}
}

public class PaymentValidator : AbstractValidator<OrderPaymentDto>
{
	public PaymentValidator()
	{
		RuleFor(x => x.Amount)
		   .GreaterThan(0)
		   .WithMessage("Payment amount must be greater than 0.");

		RuleFor(x => x.PaymentDate)
		   .NotEmpty()
		   .WithMessage("Payment date cannot be empty.")
		   .LessThanOrEqualTo(DateTime.UtcNow)
		   .WithMessage("Payment date cannot be in the future.");

		RuleFor(x => x.PaymentMethod)
		   .IsInEnum()
		   .WithMessage("Invalid payment method.");

		RuleFor(x => x.TransactionId)
		   .NotEmpty()
		   .WithMessage("Transaction ID cannot be empty.")
		   .Length(6, 50)
		   .WithMessage("Transaction ID must be between 6 and 50 characters long.");
	}
}