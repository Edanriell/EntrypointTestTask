using FluentValidation;

namespace Server.Application.Orders.ReturnOrder;

public sealed class ReturnOrderCommandValidator : AbstractValidator<ReturnOrderCommand>
{
    public ReturnOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.ReturnReason)
            .NotEmpty()
            .WithMessage("Return reason is required")
            .MaximumLength(500)
            .WithMessage("Return reason cannot exceed 500 characters");
        // Silly example
        // RuleFor(x => x.ReturnItems)
        //     .NotEmpty()
        //     .WithMessage("At least one item must be specified for return");
        //
        // RuleForEach(x => x.ReturnItems)
        //     .SetValidator(new ReturnItemValidator());
        //
        // RuleFor(x => x.CustomerComments)
        //     .MaximumLength(1000)
        //     .WithMessage("Customer comments cannot exceed 1000 characters")
        //     .When(x => !string.IsNullOrEmpty(x.CustomerComments));
        //
        // RuleFor(x => x.RequestedBy)
        //     .MaximumLength(100)
        //     .WithMessage("Requested by cannot exceed 100 characters")
        //     .When(x => !string.IsNullOrEmpty(x.RequestedBy));
    }
}

// Just an silly example
// public sealed class ReturnItemValidator : AbstractValidator<ReturnItem>
// {
//     public ReturnItemValidator()
//     {
//         RuleFor(x => x.ProductId)
//             .NotEmpty()
//             .WithMessage("Product ID is required");
//
//         RuleFor(x => x.Quantity)
//             .GreaterThan(0)
//             .WithMessage("Return quantity must be greater than 0");
//
//         RuleFor(x => x.ItemCondition)
//             .MaximumLength(100)
//             .WithMessage("Item condition cannot exceed 100 characters")
//             .When(x => !string.IsNullOrEmpty(x.ItemCondition));
//
//         RuleFor(x => x.ItemNotes)
//             .MaximumLength(500)
//             .WithMessage("Item notes cannot exceed 500 characters")
//             .When(x => !string.IsNullOrEmpty(x.ItemNotes));
//     }
// }
