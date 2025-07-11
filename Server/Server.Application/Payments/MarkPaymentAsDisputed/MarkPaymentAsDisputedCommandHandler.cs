using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Payments;

namespace Server.Application.Payments.MarkPaymentAsDisputed;

internal sealed class MarkPaymentAsDisputedCommandHandler : ICommandHandler<MarkPaymentAsDisputedCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkPaymentAsDisputedCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkPaymentAsDisputedCommand request, CancellationToken cancellationToken)
    {
        Payment? payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure(PaymentErrors.NotFound);
        }

        Result disputeResult = payment.MarkAsDisputed(request.DisputeReason);
        if (disputeResult.IsFailure)
        {
            return disputeResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
