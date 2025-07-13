using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Payments;
using Server.Domain.Shared;

namespace Server.Application.Payments.CreatePayment;

internal sealed class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure<Guid>(OrderErrors.NotFound);
        }

        Result<Currency> currencyResult = Currency.FromCode(request.Currency);
        if (currencyResult.IsFailure)
        {
            return Result.Failure<Guid>(currencyResult.Error);
        }

        Result<PaymentMethod> paymentMethodResult = PaymentMethodExtensions.FromString(request.PaymentMethod);
        if (paymentMethodResult.IsFailure)
        {
            return Result.Failure<Guid>(paymentMethodResult.Error);
        }

        var amount = new Money(request.Amount, currencyResult.Value);

        Result<Payment> paymentResult = Payment.Create(
            request.OrderId,
            amount,
            paymentMethodResult.Value,
            request.PaymentReference);

        if (paymentResult.IsFailure)
        {
            return Result.Failure<Guid>(paymentResult.Error);
        }

        Payment payment = paymentResult.Value;

        // Add payment to order
        Result addPaymentResult = order.AddPayment(payment);
        if (addPaymentResult.IsFailure)
        {
            return Result.Failure<Guid>(addPaymentResult.Error);
        }

        _paymentRepository.Add(payment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}
