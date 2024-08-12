using Application.Interfaces;
using Application.Orders.Dto;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;

namespace Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand : IRequest<IResult>
{
	public string                         UserId                   { get; set; } = string.Empty;
	public string                         ShippingName             { get; set; } = string.Empty;
	public AddressDto                     ShippingAddress          { get; set; } = new();
	public AddressDto                     BillingAddress           { get; set; } = new();
	public string?                        AdditionalInformation    { get; set; }
	public List<OrderPaymentDto>?         Payments                 { get; set; }
	public List<ProductIdWithQuantityDto> ProductIdsWithQuantities { get; set; } = null!;
}

public class CreateOrderCommandHandler(IApplicationDbContext context, IMapper mapper)
	: IRequestHandler<CreateOrderCommand, IResult>
{
	public async Task<IResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
	{
		var entity = new Order
					 {
						 UserId       = request.UserId,
						 CreatedAt    = DateTime.UtcNow,
						 UpdatedAt    = DateTime.UtcNow,
						 Status       = OrderStatus.Created,
						 ShippingName = request.ShippingName,
						 ShippingAddress = new Address
										   {
											   City       = request.ShippingAddress.City,
											   Country    = request.ShippingAddress.Country,
											   Region     = request.ShippingAddress.Region,
											   Street     = request.ShippingAddress.Street,
											   PostalCode = request.ShippingAddress.PostalCode
										   },
						 BillingAddress = new Address
										  {
											  City       = request.BillingAddress.City,
											  Country    = request.BillingAddress.Country,
											  Region     = request.BillingAddress.Region,
											  Street     = request.BillingAddress.Street,
											  PostalCode = request.BillingAddress.PostalCode
										  },
						 AdditionalInformation = request.AdditionalInformation,
						 Payments = request.Payments?.Select(payment => new Payment
																		{
																			Amount        = payment.Amount,
																			PaymentDate   = payment.PaymentDate,
																			PaymentMethod = payment.PaymentMethod,
																			TransactionId = payment.TransactionId
																		}).ToList(),
						 OrderProducts = request.ProductIdsWithQuantities
							.Select(
									 product =>
										 new ProductOrderLink
										 {
											 ProductId = product.ProductId,
											 Quantity  = product.Quantity
										 }
								 )
							.ToList()
					 };

		entity.AddDomainEvent(new OrderCreatedEvent(entity));

		context.Orders.Add(entity);
		await context.SaveChangesAsync(cancellationToken);

		var savedOrder = await context.Orders
							.AsNoTracking()
							.Where(order => order.Id == entity.Id)
							.AsSplitQuery()
							.Include(order => order.User)
							.Include(order => order.OrderProducts)!
							.ThenInclude(productOrderLink => productOrderLink.Product)
							.Include(order => order.Payments)
							.ProjectTo<CreateOrderCommandResponseDto>(mapper.ConfigurationProvider)
							.FirstOrDefaultAsync(cancellationToken);

		if (savedOrder is null) return TypedResults.Problem("Could not find created order in database.");

		return TypedResults.Ok(savedOrder);
	}
}