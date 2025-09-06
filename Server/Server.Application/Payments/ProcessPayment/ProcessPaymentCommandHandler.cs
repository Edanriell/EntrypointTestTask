using Server.Application.Abstractions.Messaging;
using Server.Domain.Abstractions;
using Server.Domain.Payments;

namespace Server.Application.Payments.ProcessPayment;

internal sealed class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand>
{
    private readonly PaymentService _paymentService;

    public ProcessPaymentCommandHandler(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        return await _paymentService.ProcessAsync(request.PaymentId, cancellationToken);
    }
}
