using Application.Interfaces;
using Domain.Events;

namespace Application.Services.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<IResult>
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public short UnitsInStock { get; set; }
    public short UnitsOnOrder { get; set; } = 0;
}

public class UpdateProductCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateProductCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        if (context.Products is null) return TypedResults.NotFound("No products in database has been found");

        var entity
            = await context.Products.FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        entity.Code = request.Code;
        entity.ProductName = request.ProductName;
        entity.Description = request.Description;
        entity.UnitPrice = request.UnitPrice;
        entity.UnitsInStock = request.UnitsInStock;
        entity.UnitsOnOrder = request.UnitsOnOrder;

        context.Products.Update(entity);

        entity.AddDomainEvent(new ProductUpdatedEvent(entity));

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}