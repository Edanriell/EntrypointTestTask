using Application.Interfaces;

namespace Application.Services.Products.Commands.WipeOutAllProducts;

public record WipeOutAllProductsCommand : IRequest<IResult>;

public class WipeOutAllProductsCommandHandler(IApplicationDbContext context)
    : IRequestHandler<WipeOutAllProductsCommand, IResult>
{
    public async Task<IResult> Handle(WipeOutAllProductsCommand request, CancellationToken cancellationToken)
    {
        if (context.Products is null) return TypedResults.NotFound("No products has been found");

        var entities = await context.Products.ToListAsync(cancellationToken);

        context.Products.RemoveRange(entities);

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}