﻿using FluentValidation;

namespace Server.Application.Products.UpdateProduct;

internal sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Product name must not exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Product description must not exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than 0")
            .When(x => x.Price.HasValue);

        RuleFor(x => x)
            .Must(HaveAtLeastOneUpdateField)
            .WithMessage("At least one field must be provided for update");
    }

    private static bool HaveAtLeastOneUpdateField(UpdateProductCommand command)
    {
        return !string.IsNullOrWhiteSpace(command.Name) ||
            !string.IsNullOrWhiteSpace(command.Description) ||
            command.Price.HasValue ||
            command.StockChange.HasValue ||
            command.ReservedChange.HasValue;
    }
}
