using Application.Interfaces;
using Application.Orders.Dto;
using Domain.Entities;
using Domain.Enums;

namespace Application.Orders.Queries.GetPaginatedSortedAndFilteredOrders;

public record GetPaginatedSortedAndFilteredOrdersQuery
	: IRequest<IResult>
{
	public int            PageIndex         { get; init; } = 0;
	public int            PageSize          { get; init; } = 10;
	public string         SortColumn        { get; init; } = "Id";
	public string         SortOrder         { get; init; } = "DESC";
	public OrderStatus?   OrderStatus       { get; set; }
	public DateTime?      CreatedAfter      { get; set; }
	public DateTime?      CreatedBefore     { get; set; }
	public DateTime?      PaymentDateAfter  { get; set; }
	public DateTime?      PaymentDateBefore { get; set; }
	public string?        CustomerName      { get; set; }
	public string?        CustomerEmail     { get; set; }
	public string?        CustomerAddress   { get; set; }
	public string?        ProductName       { get; set; }
	public decimal?       MinAmount         { get; set; }
	public decimal?       MaxAmount         { get; set; }
	public PaymentMethod? PaymentMethod     { get; set; }
}

public class GetPaginatedSortedAndFilteredOrdersQueryHandler(
	IApplicationDbContext context,
	IMapper               mapper,
	IMemoryCache          memoryCache)
	: IRequestHandler<GetPaginatedSortedAndFilteredOrdersQuery, IResult>
{
	public async Task<IResult> Handle(
			GetPaginatedSortedAndFilteredOrdersQuery request,
			CancellationToken                        cancellationToken
		)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, OrderDto[] result) dataTuple))
		{
			var query = context.Orders
			   .AsNoTracking()
			   .Include(order => order.User)
			   .Include(order => order.OrderProducts)!
			   .ThenInclude(productOrderLink => productOrderLink.Product)
			   .Include(order => order.Payments)
			   .AsSplitQuery();

			var predicate = PredicateBuilder.New<Order>(true);

			#region Custom Orders Filters

			if (request.OrderStatus is not null)
				predicate = predicate.And(
						order => order.Status == request.OrderStatus.Value
					);

			if (request.CreatedAfter.HasValue)
				predicate = predicate.And(order => order.CreatedAt >= request.CreatedAfter.Value);

			if (request.CreatedBefore.HasValue)
				predicate = predicate.And(order => order.CreatedAt <= request.CreatedBefore.Value);

			if (request.PaymentDateAfter.HasValue)
				predicate = predicate.And(order =>
					order.Payments!.Any(payment => payment.PaymentDate >= request.PaymentDateAfter.Value));

			if (request.PaymentDateBefore.HasValue)
				predicate = predicate.And(order =>
					order.Payments!.Any(payment => payment.PaymentDate <= request.PaymentDateBefore.Value));

			if (!string.IsNullOrWhiteSpace(request.CustomerName))
				predicate = predicate.And(order =>
					(order.User.Name + " " + order.User.Surname).ToLower()
				   .Contains(request.CustomerName.ToLower()));

			if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
				predicate = predicate.And(order =>
					order.User.Email!.ToLower().Contains(request.CustomerEmail.ToLower()));

			if (!string.IsNullOrWhiteSpace(request.CustomerAddress))
				predicate = predicate.And(order =>
					(order.ShippingAddress.Country + " " + order.ShippingAddress.Region + " " +
					 order.ShippingAddress.City    + " " + order.ShippingAddress.Street + " " +
					 order.ShippingAddress.PostalCode).ToLower().Contains(request.CustomerAddress.ToLower()));

			if (!string.IsNullOrWhiteSpace(request.ProductName))
				predicate = predicate.And(order =>
					order.OrderProducts != null && order.OrderProducts.Any(productOrderLink =>
						productOrderLink.Product.Name.ToLower()
						   .Contains(request.ProductName.ToLower())));

			if (request.MinAmount.HasValue)
				predicate = predicate.And(order =>
					order.Payments!.Sum(payment => payment.Amount) >= request.MinAmount.Value);

			if (request.MaxAmount.HasValue)
				predicate = predicate.And(order =>
					order.Payments!.Sum(payment => payment.Amount) <= request.MaxAmount.Value);

			if (request.PaymentMethod is not null)
				predicate = predicate.And(order =>
					order.Payments!.Any(payment => payment.PaymentMethod == request.PaymentMethod.Value));

			#endregion

			dataTuple.recordCount = await query.Where(predicate).CountAsync(cancellationToken);

			if (dataTuple.recordCount == 0)
				return TypedResults.NotFound(new { Message = "No orders found matching the criteria." });

			query = query
			   .Where(predicate)
			   .OrderBy($"{request.SortColumn} {request.SortOrder}")
			   .Skip(request.PageIndex * request.PageSize)
			   .Take(request.PageSize);

			dataTuple.result =
				await query.ProjectTo<OrderDto>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 1, 0));
		}

		return TypedResults.Ok(
				new GetPaginatedSortedAndFilteredOrdersQueryResponseDto<OrderDto[]>
				{
					Data        = dataTuple.result,
					PageIndex   = request.PageIndex,
					PageSize    = request.PageSize,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}