using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Payments;
using Server.Domain.Refunds;

namespace Server.Application.Payments.RefundPayments;

internal sealed class RefundPaymentsCommandHandler : ICommandHandler<RefundPaymentsCommand>
{
    private readonly OrderPaymentService _orderPaymentService;
    private readonly IUnitOfWork _unitOfWork;

    public RefundPaymentsCommandHandler(
        OrderPaymentService orderPaymentService,
        IUnitOfWork unitOfWork)
    {
        _orderPaymentService = orderPaymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RefundPaymentsCommand request, CancellationToken cancellationToken)
    {
        Result<RefundReason> reasonResult = RefundReason.Create(request.RefundReason);
        if (reasonResult.IsFailure)
        {
            return Result.Failure(reasonResult.Error);
        }

        Result refundResult = await _orderPaymentService.ProcessFullRefundForOrderAsync(
            request.OrderId,
            reasonResult.Value,
            cancellationToken);
        if (refundResult.IsFailure)
        {
            return Result.Failure(refundResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
