using Application.Interfaces;
using Domain.Enums;
using Domain.Events;

namespace Application.Services.Orders.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest<IResult>
{
    public int Id { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string? ShipAddress { get; set; }
    public string? OrderInformation { get; set; }
}

public class UpdateOrderCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateOrderCommand, IResult>
{
    public async Task<IResult> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        if (context.Orders is null) return TypedResults.NotFound("No orders in database has been found");

        var entity
            = await context.Orders.FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Status = request.OrderStatus;
        entity.OrderInformation = request.OrderInformation!;
        entity.ShipAddress = request.ShipAddress!;
        entity.UpdatedAt = DateTime.UtcNow;

        context.Orders.Update(entity);

        entity.AddDomainEvent(new OrderUpdatedEvent(entity));

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}