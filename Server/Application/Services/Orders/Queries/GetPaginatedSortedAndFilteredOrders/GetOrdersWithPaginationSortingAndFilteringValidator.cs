using Order = Domain.Entities.Order;

namespace Application.Services.Orders.Queries.GetPaginatedSortedAndFilteredOrders;

public class GetOrdersWithPaginationSortingAndFilteringValidator
    : AbstractValidator<GetPaginatedSortedAndFilteredOrdersQuery>
{
    public GetOrdersWithPaginationSortingAndFilteringValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Page index must be greater than or equal to 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Page index cant be greater than 100");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page size must be greater than or equal to 1")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cant be greater than 100");

        RuleFor(x => x.SortColumn)
            .Length(2, 24)
            .WithMessage("Sort column name must be at least 2 chars and no more than 24 chars long")
            .Must(BeValidSortColumn)
            .WithMessage("Chosen sort column is not present in order entity");

        RuleFor(x => x.SortOrder)
            .Length(3, 4)
            .WithMessage("Sort order name type must be at least 3 chars and no more than 4 chars long")
            .Must(BeValidSortOrder)
            .WithMessage("Sort order can be only ASC or DESC");

        RuleFor(x => x.OrderStatus)
            .IsInEnum();

        RuleFor(x => x.OrderShipAddress)
            .MinimumLength(6)
            .WithMessage("Order ship address must be at least 6 characters long")
            .MaximumLength(100)
            .WithMessage("Order ship address cant be longer than 60 characters");

        RuleFor(x => x.OrderOrdererUserName)
            .Length(3, 36)
            .WithMessage(
                "Order orderer user name must be at least 3 characters long, and no longer than 36 characters");

        RuleFor(x => x.OrderOrdererUserSurname)
            .Length(3, 36)
            .WithMessage(
                "Order orderer user surname must be at least 3 characters long, and no longer than 36 characters");

        RuleFor(x => x.OrderOrdererUserEmail)
            .EmailAddress()
            .WithMessage("Order orderer user email must be valid email");

        RuleFor(x => x.OrderProductName)
            .Length(4, 64)
            .WithMessage("Order product name must be at least 4 characters long, and no longer than 64 characters");
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