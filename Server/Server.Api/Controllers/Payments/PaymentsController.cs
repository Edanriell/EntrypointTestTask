using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Payments.GetPaymentById;
using Server.Application.Payments.ProcessPaymentWithAutomaticOrderConfirmation;
using Server.Application.Payments.RefundPaymentWithOrderUpdate;
using Server.Domain.Abstractions;

namespace Server.Api.Controllers.Payments;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/payments")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    ///     Get payment by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPaymentById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentByIdQuery(id);

        Result<PaymentResponse> result = await _sender.Send(
            query,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    ///     Process payment with automatic order confirmation
    /// </summary>
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPaymentWithAutomaticOrderConfirmation(
        ProcessPaymentWithAutomaticOrderConfirmationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentWithAutomaticOrderConfirmationCommand
        {
            OrderId = request.OrderId,
            PaymentAmount = request.PaymentAmount
        };

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    ///     Refund payment with order update
    /// </summary>
    [HttpPost("{id:guid}/refund")]
    public async Task<IActionResult> RefundPaymentWithOrderUpdate(
        Guid id,
        RefundPaymentWithOrderUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefundPaymentWithOrderUpdateCommand
        {
            OrderId = id,
            RefundAmount = request.RefundAmount,
            RefundReason = request.RefundReason
        };

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
