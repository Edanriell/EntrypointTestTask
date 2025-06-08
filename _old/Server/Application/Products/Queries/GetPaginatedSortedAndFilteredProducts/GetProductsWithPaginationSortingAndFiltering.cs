using Application.Interfaces;
using Application.Products.Dto;
using Domain.Entities;

namespace Application.Products.Queries.GetPaginatedSortedAndFilteredProducts;

public record GetPaginatedSortedAndFilteredProductsQuery
	: IRequest<IResult>
{
	public int      PageIndex       { get; set; } = 0;
	public int      PageSize        { get; set; } = 10;
	public string   SortColumn      { get; set; } = "Id";
	public string   SortOrder       { get; set; } = "DESC";
	public string?  Code            { get; set; }
	public string?  ProductName     { get; set; }
	public int?     UnitsInStock    { get; set; }
	public int?     UnitsOnOrder    { get; set; }
	public string?  CustomerName    { get; set; }
	public string?  CustomerSurname { get; set; }
	public string?  CustomerEmail   { get; set; }
	public string?  Category        { get; set; }
	public decimal? MinPrice        { get; set; }
	public decimal? MaxPrice        { get; set; }
	public string?  Brand           { get; set; }
	public int?     MinRating       { get; set; }
	public int?     MaxRating       { get; set; }
}

public class GetPaginatedSortedAndFilteredProductsQueryHandler(
	IApplicationDbContext context,
	IMapper               mapper,
	IMemoryCache          memoryCache)
	: IRequestHandler<GetPaginatedSortedAndFilteredProductsQuery,
		IResult>
{
	public async Task<IResult> Handle(
			GetPaginatedSortedAndFilteredProductsQuery request,
			CancellationToken                          cancellationToken
		)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, ProductDto[] result) dataTuple))
		{
			var query = context.Products
			   .AsNoTracking()
			   .Include(product => product.ProductOrders)!
			   .ThenInclude(productOrderLink => productOrderLink.Order)
			   .ThenInclude(order => order!.User)
			   .Include(product => product.Categories)
			   .AsSplitQuery();

			var predicate = PredicateBuilder.New<Product>(true);

			#region Custom Products Filters

			if (!string.IsNullOrWhiteSpace(request.Code))
				predicate = predicate.And(product => product.Code.ToLower().Contains(request.Code.ToLower()));

			if (!string.IsNullOrWhiteSpace(request.ProductName))
				predicate = predicate.And(product => product.Name.ToLower().Contains(request.ProductName.ToLower()));

			if (request.UnitsInStock.HasValue)
				predicate = predicate.And(product => product.UnitsInStock >= request.UnitsInStock);

			if (request.UnitsOnOrder.HasValue)
				predicate = predicate.And(product => product.UnitsOnOrder >= request.UnitsOnOrder);

			if (!string.IsNullOrWhiteSpace(request.CustomerName))
				predicate = predicate.And(product => product.ProductOrders!
				   .Any(productOrderLink =>
						productOrderLink.Order!.User!.Name.ToLower().Contains(request.CustomerName.ToLower())));

			if (!string.IsNullOrWhiteSpace(request.CustomerSurname))
				predicate = predicate.And(product => product.ProductOrders!.Any(productOrderLink =>
					productOrderLink.Order!.User!.Surname.ToLower().Contains(request.CustomerSurname.ToLower())));

			if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
				predicate = predicate.And(product => product.ProductOrders!
				   .Any(productOrderLink => productOrderLink.Order!.User!.Email!.ToLower()
					   .Contains(request.CustomerEmail.ToLower())));

			if (!string.IsNullOrWhiteSpace(request.Category))
				predicate = predicate.And(product => product.ProductCategories!.Any(productCategories =>
					productCategories.Category!.Name!.ToLower().Contains(request.Category.ToLower())));

			if (request.MinPrice.HasValue)
				predicate = predicate.And(product => product.UnitPrice >= request.MinPrice.Value);

			if (request.MaxPrice.HasValue)
				predicate = predicate.And(product => product.UnitPrice <= request.MaxPrice.Value);

			if (!string.IsNullOrWhiteSpace(request.Brand))
				predicate = predicate.And(product => product.Brand.ToLower().Contains(request.Brand.ToLower()));

			if (request.MinRating.HasValue)
				predicate = predicate.And(product => product.Rating >= request.MinRating.Value);

			if (request.MaxRating.HasValue)
				predicate = predicate.And(product => product.Rating <= request.MaxRating.Value);

			#endregion

			dataTuple.recordCount = await query.Where(predicate).CountAsync(cancellationToken);

			if (dataTuple.recordCount == 0)
				return TypedResults.NotFound(new { Message = "No products found matching the criteria." });

			query = query
			   .Where(predicate)
			   .OrderBy($"{request.SortColumn} {request.SortOrder}")
			   .Skip(request.PageIndex * request.PageSize)
			   .Take(request.PageSize);

			dataTuple.result =
				await query.ProjectTo<ProductDto>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
		}

		return TypedResults.Ok(
				new GetPaginatedSortedAndFilteredProductsQueryResponseDto<ProductDto[]>
				{
					Data        = dataTuple.result,
					PageIndex   = request.PageIndex,
					PageSize    = request.PageSize,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}