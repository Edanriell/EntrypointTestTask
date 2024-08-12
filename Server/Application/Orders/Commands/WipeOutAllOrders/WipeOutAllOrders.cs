using Application.Interfaces;

namespace Application.Orders.Commands.WipeOutAllOrders;

public record WipeOutAllOrdersCommand : IRequest<IResult>;

public class WipeOutAllOrdersCommandHandler(IApplicationDbContext context)
	: IRequestHandler<WipeOutAllOrdersCommand, IResult>
{
	public async Task<IResult> Handle(WipeOutAllOrdersCommand request, CancellationToken cancellationToken)
	{
		var entities = await context.Orders.ToListAsync(cancellationToken);

		context.Orders.RemoveRange(entities);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.NoContent();
	}
}

// Warning!
// Use for development purposes only. 