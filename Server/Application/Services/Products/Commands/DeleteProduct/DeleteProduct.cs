using Application.Interfaces;
using Domain.Events;

namespace Application.Services.Products.Commands.DeleteProduct;

public record DeleteProductCommand : IRequest<IResult>
{
    public int Id { get; set; }
    public string ProductName { get; set; } = null!;
}

public class DeleteProductCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteProductCommand, IResult>
{
    public async Task<IResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        if (context.Products is null) return TypedResults.NotFound("No products has been found");

        var entity = await context.Products.FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (entity.ProductName != request.ProductName)
            return TypedResults.BadRequest("Entered wrong product name");

        context.Products.Remove(entity);

        entity.AddDomainEvent(new ProductDeletedEvent(entity));

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}