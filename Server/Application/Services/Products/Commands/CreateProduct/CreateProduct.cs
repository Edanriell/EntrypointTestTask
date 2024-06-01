using Application.Interfaces;
using Domain.Entities;
using Domain.Events;

namespace Application.Services.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<IResult>
{
    public string Code { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public short UnitsInStock { get; set; }
    public short UnitsOnOrder { get; set; } = 0;
}

public class CreateProductCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateProductCommand, IResult>
{
    public async Task<IResult> Handle(CreateProductCommand request
        , CancellationToken cancellationToken)
    {
        var entity = new Product
        {
            Code = request.Code,
            ProductName = request.ProductName,
            Description = request.Description,
            UnitPrice = request.UnitPrice,
            UnitsInStock = request.UnitsInStock,
            UnitsOnOrder = request.UnitsOnOrder
        };

        entity.AddDomainEvent(new ProductCreatedEvent(entity));

        context.Products.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        var savedProduct = await context.Products
            .AsNoTracking()
            .Where(product => product.Id == entity.Id)
            .Include(product => product.ProductOrders)!
            .ThenInclude(productOrderLink => productOrderLink.Order)
            .ProjectTo<CreateProductCommandResponseDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (savedProduct is null) return TypedResults.Problem("Could not found created product in database");

        return TypedResults.Ok(savedProduct);
    }
}