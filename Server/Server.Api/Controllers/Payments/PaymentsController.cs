using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Payments.CancelPayment;
using Server.Application.Payments.CreatePayment;
using Server.Application.Payments.FailPayment;
using Server.Application.Payments.GetPaymentById;
using Server.Application.Payments.GetPaymentsByOrderId;
using Server.Application.Payments.MarkPaymentAsDisputed;
using Server.Application.Payments.MarkPaymentAsExpired;
using Server.Application.Payments.MarkPaymentAsProcessing;
using Server.Application.Payments.ProcessPartialRefund;
using Server.Application.Payments.ProcessPayment;
using Server.Domain.Abstractions;
using PaymentResponse = Server.Application.Payments.GetPaymentById.PaymentResponse;

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

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetPaymentsByOrderId(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentsByOrderIdQuery(orderId);

        Result<IReadOnlyList<GetPaymentsByOrderIdResponse>> result = await _sender.Send(
            query,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(
        CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand(
            request.OrderId,
            request.Amount,
            request.Currency,
            request.PaymentMethod,
            request.PaymentReference);

        Result<Guid> result = await _sender.Send(
            command,
            cancellationToken
        );

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetPaymentById),
            new { id = result.Value },
            result.Value
        );
    }

    [HttpPatch("{id:guid}/processing")]
    public async Task<IActionResult> MarkPaymentAsProcessing(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new MarkPaymentAsProcessingCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/process")]
    public async Task<IActionResult> ProcessPayment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/fail")]
    public async Task<IActionResult> FailPayment(
        Guid id,
        FailPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new FailPaymentCommand(id, request.Reason);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> CancelPayment(
        Guid id,
        CancelPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CancelPaymentCommand(id, request.Reason);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/dispute")]
    public async Task<IActionResult> MarkPaymentAsDisputed(
        Guid id,
        MarkPaymentAsDisputedRequest request,
        CancellationToken cancellationToken)
    {
        var command = new MarkPaymentAsDisputedCommand(id, request.DisputeReason);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/expire")]
    public async Task<IActionResult> MarkPaymentAsExpired(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new MarkPaymentAsExpiredCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/partial-refund")]
    public async Task<IActionResult> ProcessPartialRefund(
        Guid id,
        ProcessPartialRefundRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ProcessPartialRefundCommand(
            id,
            request.RefundAmount,
            request.Currency,
            request.Reason);

        Result<Guid> result = await _sender.Send(
            command,
            cancellationToken
        );

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { RefundId = result.Value });
    }
}
