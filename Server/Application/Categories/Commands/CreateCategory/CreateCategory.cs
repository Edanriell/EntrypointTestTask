using Application.Interfaces;
using Domain.Entities;
using Domain.Events;

namespace Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand : IRequest<IResult>
{
	public string Name        { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
}

public class CreateCategoryCommandHandler(IApplicationDbContext context, IMapper mapper)
	: IRequestHandler<CreateCategoryCommand, IResult>
{
	public async Task<IResult> Handle(CreateCategoryCommand request
									, CancellationToken     cancellationToken)
	{
		var entity = new Category
					 {
						 Name        = request.Name,
						 Description = request.Description
					 };

		entity.AddDomainEvent(new CategoryCreatedEvent(entity));

		context.Categories.Add(entity);
		await context.SaveChangesAsync(cancellationToken);

		var savedProduct = await context.Categories
							  .AsNoTracking()
							  .AsSplitQuery()
							  .Where(category => category.Id == entity.Id)
							  .Include(category => category.CategoryProducts)
							  .ProjectTo<CreateCategoryResponseDto>(mapper.ConfigurationProvider)
							  .FirstOrDefaultAsync(cancellationToken);

		if (savedProduct is null) return TypedResults.Problem("Could not find created category in database");

		return TypedResults.Ok(savedProduct);
	}
}