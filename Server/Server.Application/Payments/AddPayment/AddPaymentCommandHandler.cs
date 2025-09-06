using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Payments;
using Server.Domain.Shared;

namespace Server.Application.Payments.AddPayment;

internal sealed class AddPaymentCommandHandler : ICommandHandler<AddPaymentCommand, Guid>
{
    private readonly PaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;

    public AddPaymentCommandHandler(PaymentService paymentService, IUnitOfWork unitOfWork)
    {
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        AddPaymentCommand request,
        CancellationToken cancellationToken)
    {
        // Validate currency
        Result<Currency> currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
        {
            return Result.Failure<Guid>(currencyResult.Error);
        }

        // Validate payment method 
        Result<PaymentMethod> paymentMethodResult = PaymentMethodExtensions.Create(request.PaymentMethod);
        if (paymentMethodResult.IsFailure)
        {
            return Result.Failure<Guid>(paymentMethodResult.Error);
        }

        // Create money
        Result<Money> moneyResult = Money.Create(request.Amount, currencyResult.Value);
        if (moneyResult.IsFailure)
        {
            return Result.Failure<Guid>(moneyResult.Error);
        }

        // Use PaymentService - it handles all validation internally
        Result<Payment> result = await _paymentService.AddPaymentAsync(
            request.OrderId,
            moneyResult.Value,
            paymentMethodResult.Value,
            cancellationToken);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
