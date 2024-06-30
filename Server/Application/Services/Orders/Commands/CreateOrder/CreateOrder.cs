using Application.Interfaces;
using Domain.Entities;
using Domain.Events;

namespace Application.Services.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<IResult>
{
    public string UserId { get; set; } = null!;
    public string ShipAddress { get; set; } = string.Empty;
    public string OrderInformation { get; set; } = string.Empty;
    public List<ProductIdsWithQuantitiesDto> ProductIdsWithQuantities { get; set; } = null!;
}

public class CreateOrderCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateOrderCommand, IResult>
{
    public async Task<IResult> Handle(CreateOrderCommand request
        , CancellationToken cancellationToken)
    {
        // var orderProducts = new List<ProductOrderLink>
        // {
        //     new()
        //     {
        //         ProductId = 1,
        //         Quantity = 2
        //     },
        //     new()
        //     {
        //         ProductId = 2,
        //         Quantity = 2
        //     }
        // };

        var entity = new Order
        {
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ShipAddress = request.ShipAddress,
            OrderInformation = request.OrderInformation,
            OrderProducts = request.ProductIdsWithQuantities
                .Select(
                    product =>
                        new ProductOrderLink
                        {
                            ProductId = product.ProductId,
                            Quantity = product.Quantity
                        }
                )
                .ToList()
            // OrderProducts = orderProducts
        };

        entity.AddDomainEvent(new OrderCreatedEvent(entity));

        context.Orders.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        var savedOrder = await context.Orders
            .AsNoTracking()
            .Where(order => order.Id == entity.Id)
            .Include(order => order.User)
            .Include(order => order.OrderProducts)!
            .ThenInclude(productOrderLink => productOrderLink.Product)
            .ProjectTo<CreateOrderCommandResponseDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (savedOrder is null) return TypedResults.Problem("Could not found created order in database");

        return TypedResults.Ok(savedOrder);
    }
}