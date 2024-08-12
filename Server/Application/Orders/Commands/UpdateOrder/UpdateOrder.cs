using Application.Interfaces;
using Application.Orders.Dto;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;

namespace Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest<IResult>
{
	public int                             Id                    { get; set; }
	public OrderStatus?                    OrderStatus           { get; set; }
	public string?                         AdditionalInformation { get; set; }
	public string?                         ShippingName          { get; set; }
	public AddressDto?                     ShippingAddress       { get; set; }
	public AddressDto?                     BillingAddress        { get; set; }
	public List<ProductIdWithQuantityDto>? UpdatedProducts       { get; set; }
	public List<OrderPaymentDto>?          UpdatedPayments       { get; set; }
}

public class UpdateOrderCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateOrderCommand, IResult>
{
	public async Task<IResult> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
	{
//		var entity
//			= await context.Orders.FindAsync([request.Id], cancellationToken);

		var entity = await context.Orders
						.AsSplitQuery()
						.Include(o => o.OrderProducts)
						.Include(o => o.Payments)
						.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

		if (entity is null)
			return TypedResults.NotFound($"Order with ID {request.Id} has not been found.");

		if (request.OrderStatus is not null)
			entity.Status = request.OrderStatus.Value;

		if (!string.IsNullOrEmpty(request.AdditionalInformation))
			entity.AdditionalInformation = request.AdditionalInformation;

		if (!string.IsNullOrEmpty(request.ShippingName))
			entity.ShippingName = request.ShippingName;

		if (request.ShippingAddress is not null)
			entity.ShippingAddress = new Address
									 {
										 City       = request.ShippingAddress.City,
										 Country    = request.ShippingAddress.Country,
										 Region     = request.ShippingAddress.Region,
										 Street     = request.ShippingAddress.Street,
										 PostalCode = request.ShippingAddress.PostalCode
									 };

		if (request.BillingAddress is not null)
			entity.BillingAddress = new Address
									{
										City       = request.BillingAddress.City,
										Country    = request.BillingAddress.Country,
										Region     = request.BillingAddress.Region,
										Street     = request.BillingAddress.Street,
										PostalCode = request.BillingAddress.PostalCode
									};

		if (request.UpdatedProducts is not null && request.UpdatedProducts.Any())
		{
			foreach (var updatedProduct in request.UpdatedProducts)
			{
				var existingOrderProductLink = entity.OrderProducts?
				   .FirstOrDefault(op => op.ProductId == updatedProduct.ProductId);

				if (existingOrderProductLink is not null)
					existingOrderProductLink.Quantity = updatedProduct.Quantity;
				else
					entity.OrderProducts?.Add(new ProductOrderLink
											  {
												  ProductId = updatedProduct.ProductId,
												  Quantity  = updatedProduct.Quantity
											  });
			}

			var updatedProductIds = request.UpdatedProducts.Select(up => up.ProductId).ToList();
			entity.OrderProducts?.RemoveAll(op => !updatedProductIds.Contains(op.ProductId));
		}

		if (request.UpdatedPayments is not null && request.UpdatedPayments.Any())
		{
			foreach (var updatedPayment in request.UpdatedPayments)
			{
				var existingPayment =
					entity.Payments?.FirstOrDefault(p => p.TransactionId == updatedPayment.TransactionId);

				if (existingPayment is not null)
				{
					existingPayment.Amount        = updatedPayment.Amount;
					existingPayment.PaymentDate   = updatedPayment.PaymentDate;
					existingPayment.PaymentMethod = updatedPayment.PaymentMethod;
				}
				else
				{
					entity.Payments?.Add(new Payment
										 {
											 Amount        = updatedPayment.Amount,
											 PaymentDate   = updatedPayment.PaymentDate,
											 PaymentMethod = updatedPayment.PaymentMethod,
											 TransactionId = updatedPayment.TransactionId
										 });
				}
			}

			var updatedTransactionIds = request.UpdatedPayments.Select(up => up.TransactionId).ToList();
			entity.Payments?.RemoveAll(p => !updatedTransactionIds.Contains(p.TransactionId));
		}

		//		if (request.UpdatedProducts is not null && request.UpdatedProducts.Any())
//			entity.OrderProducts = request.UpdatedProducts.Select(product => new ProductOrderLink
//																			 {
//																				 ProductId = product.ProductId,
//																				 Quantity = product.Quantity
//																			 }).ToList();
//
//		if (request.UpdatedPayments is not null && request.UpdatedPayments.Any())
//			entity.Payments = request.UpdatedPayments.Select(payment => new Payment
//																		{
//																			Amount = payment.Amount,
//																			PaymentDate = payment.PaymentDate,
//																			PaymentMethod = payment.PaymentMethod,
//																			TransactionId = payment.TransactionId
//																		}).ToList();

		entity.UpdatedAt = DateTime.UtcNow;

		context.Orders.Update(entity);

		entity.AddDomainEvent(new OrderUpdatedEvent(entity));

		await context.SaveChangesAsync(cancellationToken);

		return TypedResults.NoContent();
	}
}