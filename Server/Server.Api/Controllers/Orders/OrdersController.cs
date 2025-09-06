using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Orders.AddProductToOrder;
using Server.Application.Orders.CancelOrder;
using Server.Application.Orders.CompleteOrder;
using Server.Application.Orders.ConfirmOrder;
using Server.Application.Orders.CreateOrder;
using Server.Application.Orders.DeleteOrder;
using Server.Application.Orders.GetOrderById;
using Server.Application.Orders.GetOrderByNumber;
using Server.Application.Orders.GetOrders;
using Server.Application.Orders.MarkOrderAsDelivered;
using Server.Application.Orders.MarkOutForDelivery;
using Server.Application.Orders.MarkReadyForShipment;
using Server.Application.Orders.RemoveProductFromOrder;
using Server.Application.Orders.ReturnOrder;
using Server.Application.Orders.ShipOrder;
using Server.Application.Orders.StartProcessingOrder;
using Server.Application.Orders.UpdateOrder;
using Server.Application.Orders.UpdateOrderProductQuantity;
using Server.Domain.Abstractions;

namespace Server.Api.Controllers.Orders;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender) { _sender = sender; }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders(
        [FromQuery] GetOrdersRequest request, CancellationToken cancellationToken)
    {
        var query = new GetOrdersQuery
        {
            PageSize = request.PageSize,
            Cursor = request.Cursor,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection,
            OrderNumberFilter = request.OrderNumberFilter,
            StatusFilter = request.StatusFilter,
            MinTotalAmount = request.MinTotalAmount,
            MaxTotalAmount = request.MaxTotalAmount,
            TrackingNumberFilter = request.TrackingNumberFilter,
            CreatedAfter = request.CreatedAfter,
            CreatedBefore = request.CreatedBefore,
            ConfirmedAfter = request.ConfirmedAfter,
            ConfirmedBefore = request.ConfirmedBefore,
            ShippedAfter = request.ShippedAfter,
            ShippedBefore = request.ShippedBefore,
            DeliveredAfter = request.DeliveredAfter,
            DeliveredBefore = request.DeliveredBefore,
            MinOutstandingAmount = request.MinOutstandingAmount,
            MaxOutstandingAmount = request.MaxOutstandingAmount,
            EstimatedDeliveryAfter = request.EstimatedDeliveryAfter,
            EstimatedDeliveryBefore = request.EstimatedDeliveryBefore,
            HasPayment = request.HasPayment,
            IsFullyPaid = request.IsFullyPaid,
            HasOutstandingBalance = request.HasOutstandingBalance,
            ProductNameFilter = request.ProductNameFilter,
            ProductIdFilter = request.ProductIdFilter,
            ClientEmailFilter = request.ClientEmailFilter,
            ClientNameFilter = request.ClientNameFilter,
            PaymentStatusFilter = request.PaymentStatusFilter
        };

        Result<GetOrdersResponse> result = await _sender.Send(
            query,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);

        Result<GetOrderByIdResponse> result = await _sender.Send(
            query,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("number/{orderNumber}")]
    public async Task<IActionResult> GetOrderByNumber(
        string orderNumber,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByNumberQuery(orderNumber);

        Result<GetOrderByNumberResponse> result = await _sender.Send(
            query,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand
        {
            ClientId = request.ClientId,
            OrderNumber = request.ClientId.ToString(),
            Currency = request.Currency,
            ShippingAddress = new ShippingAddress
            {
                Country = request.ShippingAddress.Country,
                City = request.ShippingAddress.City,
                ZipCode = request.ShippingAddress.ZipCode,
                Street = request.ShippingAddress.Street
            },
            Info = request.Info,
            OrderItems = request.OrderItems.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        };

        Result<Guid> result = await _sender.Send(
            command,
            cancellationToken
        );

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetOrderById),
            new { id = result.Value },
            result.Value
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOrder(
        Guid id,
        UpdateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrder
        {
            OrderId = id,
            Street = request.Street,
            City = request.City,
            ZipCode = request.ZipCode,
            Country = request.Country,
            Info = request.Info
        };

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/confirm")]
    public async Task<IActionResult> ConfirmOrder(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmOrderCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }


    [HttpPatch("{id:guid}/start-processing")]
    public async Task<IActionResult> StartProcessingOrder(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new StartProcessingOrderCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/ready-for-shipment")]
    public async Task<IActionResult> MarkReadyForShipment(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new MarkReadyForShipmentCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/ship")]
    public async Task<IActionResult> ShipOrder(
        Guid id,
        [FromBody] ShipOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ShipOrderCommand(
            id,
            request.TrackingNumber,
            request.Courier,
            request.EstimatedDeliveryDate
        );

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }


    [HttpPatch("{id:guid}/out-for-delivery")]
    public async Task<IActionResult> MarkOutForDelivery(
        Guid id,
        CancellationToken cancellationToken,
        [FromBody] OutForDeliveryRequest? request = null)
    {
        var command = new MarkOutForDeliveryCommand(
            id,
            request?.EstimatedDeliveryDate
        );

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }


    [HttpPatch("{id:guid}/deliver")]
    public async Task<IActionResult> MarkOrderAsDelivered(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new MarkOrderAsDeliveredCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> CompleteOrder(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new CompleteOrderCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/return")]
    public async Task<IActionResult> ReturnOrder(
        Guid id,
        [FromBody] ReturnOrderRequest request, // ✅ Add [FromBody] parameter
        CancellationToken cancellationToken)
    {
        var command = new ReturnOrderCommand
        {
            OrderId = id,
            ReturnReason = request.ReturnReason // ✅ Use the request data
        };

        Result result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }


    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(
        Guid id,
        [FromBody] CancelOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CancelOrderCommand
        {
            OrderId = id,
            CancellationReason = request.CancellationReason
        };

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOrder(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteOrderCommand { OrderId = id };

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{orderId:guid}/products")]
    public async Task<IActionResult> AddProductsToOrder(
        Guid orderId,
        [FromBody] AddProductToOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddProductToOrderCommand(
            orderId,
            request.Products.Select(p => new ProductItem(p.ProductId, p.Quantity)).ToList());

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpDelete("{id:guid}/products")]
    public async Task<IActionResult> RemoveProductsFromOrder(
        Guid id,
        RemoveProductsFromOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RemoveProductFromOrderCommand
        {
            OrderId = id,
            ProductRemovals = request.ProductRemovals
                .Select(dto => new ProductRemovalRequest
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                })
                .ToList()
        };

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }


    [HttpPatch("{id:guid}/products/{productId:guid}/quantity")]
    public async Task<IActionResult> UpdateOrderProductQuantity(
        Guid id,
        Guid productId,
        UpdateOrderProductQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrderProductQuantityCommand
        {
            OrderId = id,
            ProductId = productId,
            Quantity = request.Quantity
        };

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
