using Application.Services.Orders.Commands.CreateOrder;
using Application.Services.Orders.Commands.DeleteOrder;
using Application.Services.Orders.Commands.UpdateOrder;
using Application.Services.Orders.Commands.WipeOutAllOrders;
using Application.Services.Orders.Queries.GetAllOrders;
using Application.Services.Orders.Queries.GetOrder;
using Application.Services.Orders.Queries.GetPaginatedSortedAndFilteredOrders;
using Domain.Constants;
using Web.Infrastructure;

namespace Web.Endpoints;

public class Orders : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup(this);

        routeGroupBuilder
            .MapGet(GetOrder, "{id}")
            .MapPost(CreateOrder)
            .MapGet(GetPaginatedSortedAndFilteredOrders)
            .MapGet(GetAllOrders, "all")
            .MapPut(UpdateOrder, "{id}")
            .MapDelete(DeleteOrder, "{id}")
            .MapDelete(WipeOutAllOrders);
    }

    [Authorize(Policy = Policies.CanGetOrder)]
    private Task<IResult> GetOrder(ISender sender, [AsParameters] GetOrderQuery query)
    {
        return sender.Send(query);
    }

    [Authorize(Policy = Policies.CanCreateOrder)]
    private Task<IResult> CreateOrder(ISender sender, [FromBody] CreateOrderCommand command)
    {
        return sender.Send(command);
    }

    [Authorize(Policy = Policies.CanManageOrders)]
    private Task<IResult> GetPaginatedSortedAndFilteredOrders(
        ISender sender, [AsParameters] GetPaginatedSortedAndFilteredOrdersQuery query)
    {
        return sender.Send(query);
    }

    [Authorize(Policy = Policies.CanManageOrders)]
    private Task<IResult> GetAllOrders(ISender sender,
        [AsParameters] GetAllOrdersQuery query)
    {
        return sender.Send(query);
    }

    [Authorize(Policy = Policies.CanManageOrders)]
    private async Task<IResult> UpdateOrder(ISender sender, int id, [FromBody] UpdateOrderCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        return await sender.Send(command);
    }

    [Authorize(Policy = Policies.CanDeleteOrders)]
    private async Task<IResult> DeleteOrder(ISender sender, int id, [FromBody] DeleteOrderCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        return await sender.Send(command);
    }

    [Authorize(Policy = Policies.CanDeleteOrders)]
    private Task<IResult> WipeOutAllOrders(ISender sender, [AsParameters] WipeOutAllOrdersCommand command)
    {
        return sender.Send(command);
    }
}