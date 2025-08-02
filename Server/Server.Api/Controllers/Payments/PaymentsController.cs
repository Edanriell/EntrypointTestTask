using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Payments.AddPayment;
using Server.Application.Payments.CancelPayment;
using Server.Application.Payments.GetPaymentById;
using Server.Application.Payments.GetPaymentsByOrderId;
using Server.Application.Payments.ProcessOrderRefundCommand;
using Server.Application.Payments.ProcessPayment;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPaymentById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetPaymentByIdQuery(id);

        Result<GetPaymentByIdResponse> result = await _sender.Send(
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

        // ✅ FIX: Changed to match the actual return type
        Result<GetPaymentsByOrderIdResponse> result = await _sender.Send(
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
        var command = new AddPaymentCommand(
            request.OrderId,
            request.Amount,
            request.Currency,
            request.PaymentMethod);

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

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> CancelPayment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new CancelPaymentCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("order/{orderId:guid}/refund")]
    public async Task<IActionResult> RefundOrderPayments(
        Guid orderId,
        [FromBody] RefundOrderPaymentsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefundPaymentsCommand(orderId, request.RefundReason);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
