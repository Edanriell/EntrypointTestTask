﻿using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Orders.AddProductToOrder;
using Server.Application.Orders.CancelOrder;
using Server.Application.Orders.CompleteOrder;
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
using Server.Application.Orders.UpdateOrderProductQuantity;
using Server.Application.Orders.UpdateOrderShippingAddress;
using Server.Domain.Abstractions;
using OrdersResponse = Server.Application.Orders.GetOrders.OrdersResponse;

namespace Server.Api.Controllers.Orders;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/orders")]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender) { _sender = sender; }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken)
    {
        var query = new GetOrdersQuery();

        Result<IReadOnlyList<OrdersResponse>> result = await _sender.Send(
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

        Result<Application.Orders.GetOrderById.OrdersResponse> result = await _sender.Send(
            query,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("number/{orderNumber}")]
    public async Task<IActionResult> GetOrderByNumber(
        Guid orderNumber,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByNumberQuery(orderNumber);

        Result<Application.Orders.GetOrderByNumber.OrdersResponse> result = await _sender.Send(
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
            ShippingAddress = new ShippingAddress
            {
                Country = request.ShippingAddress.Country,
                City = request.ShippingAddress.City,
                ZipCode = request.ShippingAddress.ZipCode,
                Street = request.ShippingAddress.Street
            },
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

    [HttpPost("{id:guid}/products")]
    public async Task<IActionResult> AddProductToOrder(
        Guid id,
        AddProductToOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddProductToOrderCommand
        {
            OrderId = id,
            Products = request.Products.Select(p => new ProductItem
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList()
        };

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
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
            ProductIds = request.ProductIds.ToList()
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
            NewQuantity = request.Quantity
        };

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/shipping-address")]
    public async Task<IActionResult> UpdateOrderShippingAddress(
        Guid id,
        UpdateOrderShippingAddressRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrderShippingAddressCommand
        {
            OrderId = id,
            Street = request.Street,
            City = request.City,
            ZipCode = request.ZipCode,
            Country = request.Country
        };

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
        ShipOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ShipOrderCommand(id, request.TrackingNumber);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/out-for-delivery")]
    public async Task<IActionResult> MarkOutForDelivery(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new MarkOutForDeliveryCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
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

    [HttpPost("{id:guid}/return")]
    public async Task<IActionResult> ReturnOrder(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ReturnOrderCommand { OrderId = id };

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new CancelOrderCommand { OrderId = id };

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
}

// POST /api/v1/orders                     → Create order (Pending)
// PATCH /api/v1/orders/{id}/confirm       → Confirm order (Confirmed)
// PATCH /api/v1/orders/{id}/start-processing → Start processing (Processing)
// PATCH /api/v1/orders/{id}/ready-for-shipment → Ready for shipment (ReadyForShipment)
// PATCH /api/v1/orders/{id}/ship          → Ship order (Shipped)
// PATCH /api/v1/orders/{id}/out-for-delivery → Out for delivery (OutForDelivery)
// PATCH /api/v1/orders/{id}/deliver       → Mark as delivered (Delivered)
// PATCH /api/v1/orders/{id}/complete      → Complete order (Completed)
// // Exception flows:
// PATCH /api/v1/orders/{id}/cancel        → Cancel order (Cancelled)
// POST /api/v1/orders/{id}/return         → Return order (Returned)
