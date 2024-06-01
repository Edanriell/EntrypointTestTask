using Application.Interfaces;
using Domain.Events;

namespace Application.Services.Orders.Commands.DeleteOrder;

public record DeleteOrderCommand : IRequest<IResult>
{
    public int Id { get; set; }
}

public class DeleteOrderCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteOrderCommand, IResult>
{
    public async Task<IResult> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        if (context.Orders is null) return TypedResults.NotFound("No orders has been found");

        var entity = await context.Orders.FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        context.Orders.Remove(entity);

        entity.AddDomainEvent(new OrderDeletedEvent(entity));

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}