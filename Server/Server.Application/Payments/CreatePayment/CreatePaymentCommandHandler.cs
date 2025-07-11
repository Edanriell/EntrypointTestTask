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
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        // Get the order
        Order? order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure<Guid>(OrderErrors.NotFound);
        }

        // Create currency
        var currency = Currency.FromCode(request.Currency);

        // Create money
        Result<Money> moneyResult = Money.Create(request.Amount, currency);
        if (moneyResult.IsFailure)
        {
            return Result.Failure<Guid>(moneyResult.Error);
        }

        // Create payment
        Result<Payment> paymentResult = Payment.Create(
            request.OrderId,
            moneyResult.Value,
            request.PaymentMethod,
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

        // Add to repository
        _paymentRepository.Add(payment);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}
