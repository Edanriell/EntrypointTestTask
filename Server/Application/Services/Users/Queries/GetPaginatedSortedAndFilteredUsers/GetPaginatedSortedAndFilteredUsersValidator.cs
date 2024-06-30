using System.Text.RegularExpressions;
using Domain.Entities;

namespace Application.Services.Users.Queries.GetPaginatedSortedAndFilteredUsers;

public class GetPaginatedSortedAndFilteredUsersValidator
    : AbstractValidator<GetPaginatedSortedAndFilteredUsersQuery>
{
    public GetPaginatedSortedAndFilteredUsersValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Page index must be greater than or equal to 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Page index can't be greater than 100");

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

        RuleFor(x => x.UserName)
            .Length(3, 50)
            .WithMessage("Username must be between 3 and 50 characters.");

        RuleFor(x => x.Name)
            .Length(2, 50)
            .WithMessage("Name must be between 2 and 50 characters.");

        RuleFor(x => x.Surname)
            .Length(2, 50)
            .WithMessage("Surname must be between 2 and 50 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("A valid email is required.");

        // RuleFor(x => x.OrderCreatedAt)
        //     .NotNull()
        //     .WithMessage("Order creation date is required.");
        //
        // RuleFor(x => x.OrderUpdatedAt)
        //     .GreaterThanOrEqualTo(x => x.OrderCreatedAt)
        //     .WithMessage("Order update date must be greater than or equal to the order creation date.");

        RuleFor(x => x.OrderStatus)
            .IsInEnum()
            .WithMessage("A valid order status is required.");

        RuleFor(x => x.OrderProductCode)
            .Matches(new Regex("^[A-Za-z0-9-]+$"))
            .WithMessage("Order product code must be alphanumeric and can include hyphens.");

        RuleFor(x => x.OrderProductName)
            .Length(2, 100)
            .WithMessage("Order product name must be between 2 and 100 characters.");
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

// CreatedAt and UpdatedAt is broken need to fix