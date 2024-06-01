using Application.Interfaces;

namespace Application.Services.Orders.Commands.WipeOutAllOrders;

public record WipeOutAllOrdersCommand : IRequest<IResult>;

public class WipeOutAllOrdersCommandHandler(IApplicationDbContext context)
    : IRequestHandler<WipeOutAllOrdersCommand, IResult>
{
    public async Task<IResult> Handle(WipeOutAllOrdersCommand request, CancellationToken cancellationToken)
    {
        if (context.Orders is null) return TypedResults.NotFound("No orders has been found");

        var entities = await context.Orders.ToListAsync(cancellationToken);

        context.Orders.RemoveRange(entities);

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}