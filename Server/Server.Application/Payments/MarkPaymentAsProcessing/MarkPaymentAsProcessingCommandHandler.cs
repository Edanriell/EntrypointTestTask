using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Payments;

namespace Server.Application.Payments.MarkPaymentAsProcessing;

internal sealed class MarkPaymentAsProcessingCommandHandler : ICommandHandler<MarkPaymentAsProcessingCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkPaymentAsProcessingCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkPaymentAsProcessingCommand request, CancellationToken cancellationToken)
    {
        Payment? payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment is null)
        {
            return Result.Failure(PaymentErrors.NotFound);
        }

        Result processingResult = payment.MarkAsProcessing();
        if (processingResult.IsFailure)
        {
            return processingResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
