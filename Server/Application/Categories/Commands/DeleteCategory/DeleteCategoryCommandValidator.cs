using Application.Interfaces;
using Domain.Events;

namespace Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand : IRequest<IResult>
{
	public int    Id           { get; set; }
	public string CategoryName { get; set; } = string.Empty;
}

public class DeleteCategoryCommandHandler(IApplicationDbContext context)
	: IRequestHandler<DeleteCategoryCommand, IResult>
{
	public async Task<IResult> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
	{
		var entity = await context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);

		if (entity is null)
			return TypedResults.NotFound($"Category with ID {request.Id} has not been found.");

		if (entity.Name != request.CategoryName)
			return TypedResults.BadRequest("Provided wrong category name.");

		context.Categories.Remove(entity);

		entity.AddDomainEvent(new CategoryDeletedEvent(entity));

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.NoContent();
	}
}