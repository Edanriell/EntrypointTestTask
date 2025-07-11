using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Orders;
using Server.Domain.Payments;
using Server.Domain.Shared;

namespace Server.Application.Payments.ProcessPartialRefund;

internal sealed class ProcessPartialRefundCommandHandler : ICommandHandler<ProcessPartialRefundCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPartialRefundCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessPartialRefundCommand request, CancellationToken cancellationToken)
    {
        Payment? payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure<Guid>(PaymentErrors.NotFound);
        }

        Result<Currency> currencyResult = Currency.FromCode(request.Currency);
        if (currencyResult.IsFailure)
        {
            return Result.Failure<Guid>(currencyResult.Error);
        }

        var refundAmount = new Money(request.RefundAmount, currencyResult.Value);

        Result<RefundReason> reasonResult = RefundReason.Create(request.Reason);
        if (reasonResult.IsFailure)
        {
            return Result.Failure<Guid>(reasonResult.Error);
        }

        Result<Refund> refundResult = payment.ProcessRefund(refundAmount, reasonResult.Value);
        if (refundResult.IsFailure)
        {
            return Result.Failure<Guid>(refundResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(refundResult.Value.Id);
    }
}
