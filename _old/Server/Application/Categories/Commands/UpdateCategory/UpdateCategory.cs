using Application.Interfaces;
using Domain.Events;

namespace Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<IResult>
{
	public int    Id                  { get; set; }
	public string CategoryName        { get; set; } = string.Empty;
	public string CategoryDescription { get; set; } = string.Empty;
}

public class UpdateCategoryCommandHandler(IApplicationDbContext context)
	: IRequestHandler<UpdateCategoryCommand, IResult>
{
	public async Task<IResult> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
	{
		var entity = await context.Categories
						.FirstOrDefaultAsync(category => category.Id == request.Id, cancellationToken);

		if (entity is null)
			return TypedResults.NotFound($"Category with ID {request.Id} has not been found.");

		if (!string.IsNullOrWhiteSpace(request.CategoryName))
			entity.Name = request.CategoryName;

		if (!string.IsNullOrWhiteSpace(request.CategoryDescription))
			entity.Description = request.CategoryDescription;

		context.Categories.Update(entity);

		entity.AddDomainEvent(new CategoryUpdatedEvent(entity));

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.NoContent();
	}
}