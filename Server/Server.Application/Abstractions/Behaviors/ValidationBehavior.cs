using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Server.Application.Abstractions.Messaging;
using Server.Application.Exceptions;
using ValidationException = Server.Application.Exceptions.ValidationException;

namespace Server.Application.Abstractions.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) { _validators = validators; }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(
            request
        );

        // Execute all validations concurrently
        IEnumerable<Task<ValidationResult>> validationTasks = _validators.Select(validator =>
            validator.ValidateAsync(context, cancellationToken)
        );

        // Wait for all validations to complete
        ValidationResult[] validationResults = await Task.WhenAll(validationTasks);

        // Process the results
        var validationErrors = validationResults
            .Where(validationResult => validationResult.Errors.Any())
            .SelectMany(validationResult => validationResult.Errors)
            .Select(validationFailure => new ValidationError(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage
            ))
            .ToList();

        if (validationErrors.Any())
        {
            throw new ValidationException(
                validationErrors
            );
        }

        return await next(cancellationToken);
    }
}
