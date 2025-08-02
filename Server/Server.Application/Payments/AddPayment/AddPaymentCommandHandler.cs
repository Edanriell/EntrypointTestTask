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


    // private readonly IOrderRepository _orderRepository;
    // private readonly IPaymentRepository _paymentRepository;
    // private readonly IUnitOfWork _unitOfWork;
    //
    // public AddPaymentCommandHandler(
    //     IPaymentRepository paymentRepository,
    //     IOrderRepository orderRepository,
    //     IUnitOfWork unitOfWork)
    // {
    //     _paymentRepository = paymentRepository;
    //     _orderRepository = orderRepository;
    //     _unitOfWork = unitOfWork;
    // }
    //
    // public async Task<Result<Guid>> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
    // {
    //     Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
    //     if (order is null)
    //     {
    //         return Result.Failure<Guid>(OrderErrors.NotFound);
    //     }
    //
    //     Result<Currency> currencyResult = Currency.FromCode(request.Currency);
    //     if (currencyResult.IsFailure)
    //     {
    //         return Result.Failure<Guid>(currencyResult.Error);
    //     }
    //
    //     var amount = new Money(request.Amount, currencyResult.Value);
    //
    //     Result<PaymentMethod> paymentMethodResult = PaymentMethodExtensions.FromString(request.PaymentMethod);
    //     if (paymentMethodResult.IsFailure)
    //     {
    //         return Result.Failure<Guid>(paymentMethodResult.Error);
    //     }
    //
    //     Result<Payment> paymentResult = Payment.Create(
    //         request.OrderId,
    //         amount,
    //         paymentMethodResult.Value,
    //         request.PaymentReference);
    //
    //     if (paymentResult.IsFailure)
    //     {
    //         return Result.Failure<Guid>(paymentResult.Error);
    //     }
    //
    //     Payment payment = paymentResult.Value;
    //
    //     // Add payment to order
    //     Result addPaymentResult = order.AddPayment(payment);
    //     if (addPaymentResult.IsFailure)
    //     {
    //         return Result.Failure<Guid>(addPaymentResult.Error);
    //     }
    //
    //     _paymentRepository.Add(payment);
    //
    //     await _unitOfWork.SaveChangesAsync(cancellationToken);
    //
    //     return Result.Success(payment.Id);
    // }
}
