using Domain.Entities;

namespace Application.Services.Products.Queries.GetPaginatedSortedAndFilteredProducts;

public class GetProductsWithPaginationSortingAndFilteringValidator
    : AbstractValidator<GetPaginatedSortedAndFilteredProductsQuery>
{
    public GetProductsWithPaginationSortingAndFilteringValidator()
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

        RuleFor(x => x.Code)
            .MinimumLength(4)
            .WithMessage("Product code must be at least 4 characters long");

        RuleFor(x => x.ProductName)
            .MinimumLength(4)
            .WithMessage("Product name must be at least 4 characters long")
            .MaximumLength(64)
            .WithMessage("Product name can't contain more than 64 characters");

        RuleFor(x => x.UnitsInStock)
            .GreaterThanOrEqualTo((short)1)
            .WithMessage("Units in stock must be at greater or equal to 1");

        RuleFor(x => x.UnitsOnOrder)
            .GreaterThanOrEqualTo((short)1)
            .WithMessage("Units in stock must be at greater or equal to 1");

        RuleFor(x => x.CustomerName)
            .MinimumLength(4)
            .WithMessage("Customer name must be at least 4 characters long")
            .MaximumLength(24)
            .WithMessage("Customer name can't contain more than 24 characters");

        RuleFor(x => x.CustomerSurname)
            .MinimumLength(4)
            .WithMessage("Customer surname must be at least 4 characters long")
            .MaximumLength(48)
            .WithMessage("Customer surname can't contain more than 48 characters");

        RuleFor(x => x.CustomerEmail)
            .EmailAddress()
            .WithMessage("Customer email must be valid email address");
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