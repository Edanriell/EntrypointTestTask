using Application.Services.Products.Commands.CreateProduct;
using Application.Services.Products.Commands.DeleteProduct;
using Application.Services.Products.Commands.UpdateProduct;
using Application.Services.Products.Commands.WipeOutAllProducts;
using Application.Services.Products.Queries.GetAllProducts;
using Application.Services.Products.Queries.GetPaginatedSortedAndFilteredProducts;
using Application.Services.Products.Queries.GetProduct;
using Domain.Constants;
using Web.Infrastructure;

namespace Web.Endpoints;

public class Products : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup(this);

        routeGroupBuilder
            .MapGet(GetPaginatedSortedAndFilteredProducts)
            .MapGet(GetProduct, "{id}")
            .MapGet(GetAllProducts, "all")
            .MapPost(CreateProduct)
            .MapPut(UpdateProduct, "{id}")
            .MapDelete(DeleteProduct, "{id}")
            .MapDelete(WipeOutAllProducts);
    }

    private Task<IResult>
        GetPaginatedSortedAndFilteredProducts(ISender sender,
            [AsParameters] GetPaginatedSortedAndFilteredProductsQuery query)
    {
        return sender.Send(query);
    }

    private Task<IResult> GetProduct(ISender sender, [AsParameters] GetProductQuery query)
    {
        return sender.Send(query);
    }

    [Authorize(Policy = Policies.CanManageProducts)]
    private Task<IResult> GetAllProducts(ISender sender,
        [AsParameters] GetAllProductsQuery query)
    {
        return sender.Send(query);
    }

    [Authorize(Policy = Policies.CanManageProducts)]
    private Task<IResult> CreateProduct(ISender sender, [FromBody] CreateProductCommand command)
    {
        return sender.Send(command);
    }

    [Authorize(Policy = Policies.CanManageProducts)]
    private async Task<IResult> UpdateProduct(ISender sender, int id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        return await sender.Send(command);
    }

    [Authorize(Policy = Policies.CanDeleteProducts)]
    private async Task<IResult> DeleteProduct(ISender sender, int id, [FromBody] DeleteProductCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        return await sender.Send(command);
    }

    [Authorize(Policy = Policies.CanDeleteProducts)]
    private Task<IResult> WipeOutAllProducts(ISender sender, [AsParameters] WipeOutAllProductsCommand command)
    {
        return sender.Send(command);
    }
}