using Application.Interfaces;
using Domain.Events;

namespace Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand : IRequest<IResult>
{
	public int    Id          { get; set; }
	public string ProductName { get; set; } = string.Empty;
}

public class DeleteProductCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteProductCommand, IResult>
{
	public async Task<IResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
	{
		var entity = await context.Products.FindAsync(new object[] { request.Id }, cancellationToken);

		if (entity is null)
			return TypedResults.NotFound($"Product with ID {request.Id} has not been found.");

		if (entity.Name != request.ProductName)
			return TypedResults.BadRequest("Provided wrong product name.");

		context.Products.Remove(entity);

		entity.AddDomainEvent(new ProductDeletedEvent(entity));

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.NoContent();
	}
}