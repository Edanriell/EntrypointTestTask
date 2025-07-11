using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Products.CreateProduct;
using Server.Application.Products.DeleteProduct;
using Server.Application.Products.DiscountProduct;
using Server.Application.Products.GetProductById;
using Server.Application.Products.GetProducts;
using Server.Application.Products.RestoreProduct;
using Server.Application.Products.UpdateProduct;
using Server.Application.Products.UpdateProductPrice;
using Server.Application.Products.UpdateProductReservedStock;
using Server.Application.Products.UpdateProductStock;
using Server.Domain.Abstractions;

namespace Server.Api.Controllers.Products;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender) { _sender = sender; }

    /// <summary>
    ///     Get all products
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllProducts(
        [FromQuery] GetProductsRequest request, CancellationToken cancellationToken)
    {
        var query = new GetProductsQuery
        {
            PageSize = request.PageSize,
            Cursor = request.Cursor,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection,
            NameFilter = request.NameFilter,
            DescriptionFilter = request.DescriptionFilter,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            MinStock = request.MinStock,
            MaxStock = request.MaxStock,
            StatusFilter = request.StatusFilter,
            CreatedAfter = request.CreatedAfter,
            CreatedBefore = request.CreatedBefore,
            LastUpdatedAfter = request.LastUpdatedAfter,
            LastUpdatedBefore = request.LastUpdatedBefore,
            LastRestockedAfter = request.LastRestockedAfter,
            LastRestockedBefore = request.LastRestockedBefore,
            HasStock = request.HasStock,
            IsReserved = request.IsReserved
        };

        Result<GetProductsResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    ///     Get product by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);

        Result<Application.Products.GetProductById.ProductResponse> result = await _sender.Send(
            query,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    ///     Create a new product
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Price,
            request.Stock
        );

        Result<Guid> result = await _sender.Send(
            command,
            cancellationToken
        );

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = result.Value },
            result.Value
        );
    }

    /// <summary>
    ///     Update product
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.Reserved
        );

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    ///     Update product price
    /// </summary>
    [HttpPatch("{id:guid}/price")]
    public async Task<IActionResult> UpdateProductPrice(
        Guid id,
        UpdateProductPriceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductPriceCommand(
            id,
            request.Price
        );

        Result result = await _sender.Send(
            command,
            cancellationToken
        );


        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    ///     Update product stock
    /// </summary>
    [HttpPatch("{id:guid}/stock")]
    public async Task<IActionResult> UpdateProductStock(
        Guid id,
        UpdateProductStockRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductStockCommand(
            id,
            request.Stock
        );

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    ///     Update product reserved stock
    /// </summary>
    [HttpPatch("{id:guid}/reserved-stock")]
    public async Task<IActionResult> UpdateProductReservedStock(
        Guid id,
        UpdateProductReservedStockRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductReservedStockCommand(
            id,
            request.ReservedStock
        );

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    ///     Apply discount to product
    /// </summary>
    [HttpPatch("{id:guid}/discount")]
    public async Task<IActionResult> DiscountProduct(
        Guid id,
        DiscountProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DiscountProductCommand(
            id,
            request.NewPrice
        );

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    ///     Delete product (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    /// <summary>
    ///     Restore deleted product
    /// </summary>
    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> RestoreProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new RestoreProductCommand(id);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
