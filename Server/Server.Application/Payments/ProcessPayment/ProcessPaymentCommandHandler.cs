using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Payments;

namespace Server.Application.Payments.ProcessPayment;

internal sealed class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand>
{
    private readonly PaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentCommandHandler(PaymentService paymentService, IUnitOfWork unitOfWork)
    {
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        Result paymentResult = await _paymentService.ProcessAsync(request.PaymentId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (paymentResult.IsFailure)
        {
            return Result.Failure(paymentResult.Error);
        }

        return Result.Success();
    }
}
