using Application.Interfaces;
using Domain.Events;

namespace Application.Orders.Commands.DeleteOrder;

public record DeleteOrderCommand : IRequest<IResult>
{
	public int Id { get; set; }
}

public class DeleteOrderCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteOrderCommand, IResult>
{
	public async Task<IResult> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
	{
		var entity = await context.Orders.FindAsync(new object[] { request.Id }, cancellationToken);

		if (entity is null)
			return TypedResults.NotFound($"Order with ID {request.Id} has not been found.");

		context.Orders.Remove(entity);

		entity.AddDomainEvent(new OrderDeletedEvent(entity));

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.NoContent();
	}
}