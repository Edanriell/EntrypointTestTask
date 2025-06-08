using Application.Interfaces;

namespace Application.Categories.Commands.WipeOutAllCategories;

public record WipeOutAllCategoriesCommand : IRequest<IResult>;

public class WipeOutAllProductsCommandHandler(IApplicationDbContext context)
	: IRequestHandler<WipeOutAllCategoriesCommand, IResult>
{
	public async Task<IResult> Handle(WipeOutAllCategoriesCommand request, CancellationToken cancellationToken)
	{
		var entities = await context.Categories.ToListAsync(cancellationToken);

		context.Categories.RemoveRange(entities);

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.NoContent();
	}
}

// Warning!
// Use for development purposes only.