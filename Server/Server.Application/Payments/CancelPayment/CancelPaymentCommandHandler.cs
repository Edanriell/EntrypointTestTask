using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Payments;

namespace Server.Application.Payments.CancelPayment;

internal sealed class CancelPaymentCommandHandler : ICommandHandler<CancelPaymentCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }
 
    public async Task<Result> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
    {
        Payment? payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure(PaymentErrors.NotFound);
        }

        Result cancelResult = payment.Cancel();
        if (cancelResult.IsFailure)
        {
            return cancelResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
