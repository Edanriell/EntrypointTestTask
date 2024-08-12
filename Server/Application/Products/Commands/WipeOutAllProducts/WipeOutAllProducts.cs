using Application.Interfaces;

namespace Application.Products.Commands.WipeOutAllProducts;

public record WipeOutAllProductsCommand : IRequest<IResult>;

public class WipeOutAllProductsCommandHandler(IApplicationDbContext context)
	: IRequestHandler<WipeOutAllProductsCommand, IResult>
{
	public async Task<IResult> Handle(WipeOutAllProductsCommand request, CancellationToken cancellationToken)
	{
		var entities = await context.Products.ToListAsync(cancellationToken);

		context.Products.RemoveRange(entities);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.NoContent();
	}
}

// Warning!
// Use for development purposes only.