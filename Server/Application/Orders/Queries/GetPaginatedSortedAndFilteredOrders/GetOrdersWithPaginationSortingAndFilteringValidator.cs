using Order = Domain.Entities.Order;

namespace Application.Orders.Queries.GetPaginatedSortedAndFilteredOrders;

public class GetOrdersWithPaginationSortingAndFilteringValidator
	: AbstractValidator<GetPaginatedSortedAndFilteredOrdersQuery>
{
	public GetOrdersWithPaginationSortingAndFilteringValidator()
	{
		RuleFor(x => x.PageIndex)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Page index must be greater than or equal to 0.")
		   .LessThanOrEqualTo(100)
		   .WithMessage("Page index must not exceed 100.");

		RuleFor(x => x.PageSize)
		   .GreaterThanOrEqualTo(1)
		   .WithMessage("Page size must be greater than or equal to 1.")
		   .LessThanOrEqualTo(100)
		   .WithMessage("Page size must not exceed 100.");

		RuleFor(x => x.SortColumn)
		   .Length(2, 24)
		   .WithMessage("Sort column name must be between 2 and 24 characters long.")
		   .Must(BeValidSortColumn)
		   .WithMessage("Specified sort column is not valid for the order entity.");

		RuleFor(x => x.SortOrder)
		   .Length(3, 4)
		   .WithMessage("Sort order must be either 'ASC' or 'DESC'.")
		   .Must(BeValidSortOrder)
		   .WithMessage("Sort order must be either 'ASC' for ascending or 'DESC' for descending order.");

		RuleFor(x => x.OrderStatus)
		   .IsInEnum()
		   .WithMessage("Invalid order status value.");

		RuleFor(x => x.CreatedAfter)
		   .LessThanOrEqualTo(x => x.CreatedBefore)
		   .When(x => x.CreatedBefore.HasValue)
		   .WithMessage("CreatedAfter date cannot be later than CreatedBefore date.");

		RuleFor(x => x.CreatedBefore)
		   .GreaterThanOrEqualTo(x => x.CreatedAfter)
		   .When(x => x.CreatedAfter.HasValue)
		   .WithMessage("CreatedBefore date cannot be earlier than CreatedAfter date.");

		RuleFor(x => x.PaymentDateAfter)
		   .LessThanOrEqualTo(x => x.PaymentDateBefore)
		   .When(x => x.PaymentDateBefore.HasValue)
		   .WithMessage("PaymentDateAfter cannot be later than PaymentDateBefore.");

		RuleFor(x => x.PaymentDateBefore)
		   .GreaterThanOrEqualTo(x => x.PaymentDateAfter)
		   .When(x => x.PaymentDateAfter.HasValue)
		   .WithMessage("PaymentDateBefore cannot be earlier than PaymentDateAfter.");

		RuleFor(x => x.CustomerName)
		   .Length(3, 72)
		   .WithMessage("Customer name must be between 3 and 72 characters long.");

		RuleFor(x => x.CustomerEmail)
		   .EmailAddress()
		   .WithMessage("Please provide a valid email address.");

		RuleFor(x => x.CustomerAddress)
		   .MinimumLength(6)
		   .WithMessage("Customer address must be at least 6 characters long.")
		   .MaximumLength(100)
		   .WithMessage("Customer address cannot be longer than 100 characters.");

		RuleFor(x => x.ProductName)
		   .Length(4, 64)
		   .WithMessage("Product name must be between 4 and 64 characters long.");

		RuleFor(x => x.MinAmount)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Minimum amount must be at least 0.");

		RuleFor(x => x.MaxAmount)
		   .GreaterThanOrEqualTo(0)
		   .WithMessage("Maximum amount must be at least 0.");

		RuleFor(x => new { x.MinAmount, x.MaxAmount })
		   .Must(x => x.MinAmount == null || x.MaxAmount == null || x.MinAmount <= x.MaxAmount)
		   .WithMessage("Minimum amount cannot be greater than maximum amount.");

		RuleFor(x => x.PaymentMethod)
		   .IsInEnum()
		   .WithMessage("Invalid payment method value.");
	}

	private bool BeValidSortColumn(string sortColumn)
	{
		var orderEntityProperties = typeof(Order).GetProperties();
		return orderEntityProperties.Any(prop => prop.Name == sortColumn);
	}

	private bool BeValidSortOrder(string sortOrder)
	{
		return sortOrder switch
			   {
				   "ASC" => true, "DESC" => true, _ => false
			   };
	}
}