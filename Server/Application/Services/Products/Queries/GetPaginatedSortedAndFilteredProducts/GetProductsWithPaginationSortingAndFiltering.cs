using Application.Interfaces;
using Application.Services.Products.Dto;
using Domain.Entities;

namespace Application.Services.Products.Queries.GetPaginatedSortedAndFilteredProducts;

public record GetPaginatedSortedAndFilteredProductsQuery
    : IRequest<IResult>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
    public string? Code { get; set; }
    public string? ProductName { get; set; }
    public short? UnitsInStock { get; set; }
    public short? UnitsOnOrder { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerSurname { get; set; }
    public string? CustomerEmail { get; set; }
}

public class GetPaginatedSortedAndFilteredProductsQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IMemoryCache memoryCache)
    : IRequestHandler<GetPaginatedSortedAndFilteredProductsQuery,
        IResult>
{
    public async Task<IResult> Handle(
        GetPaginatedSortedAndFilteredProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        if (context.Products is null) return TypedResults.NotFound("No products has been found");

        var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

        if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, ProductDto[] result) dataTuple))
        {
            var query = context.Products
                .AsNoTracking()
                .Include(product => product.ProductOrders)!
                .ThenInclude(productOrderLink => productOrderLink.Order)
                .ThenInclude(order => order!.User)
                .AsSplitQuery();

            var predicate = PredicateBuilder.New<Product>(true);

            #region Custom Products Filters

            if (!string.IsNullOrWhiteSpace(request.Code))
                predicate = predicate.And(
                    product =>
                        product.Code
                            .ToLower()
                            .Contains(request.Code.ToLower())
                );

            if (!string.IsNullOrWhiteSpace(request.ProductName))
                predicate = predicate.And(
                    product =>
                        product.ProductName
                            .ToLower()
                            .Contains(request.ProductName.ToLower())
                );

            if (request.UnitsInStock >= 0)
                predicate = predicate.And(
                    product => product.UnitsInStock >= request.UnitsInStock
                );

            if (request.UnitsOnOrder >= 0)
                predicate = predicate.And(
                    product => product.UnitsOnOrder >= request.UnitsOnOrder
                );

            if (!string.IsNullOrWhiteSpace(request.CustomerName))
                predicate = predicate.And(
                    product =>
                        product.ProductOrders!.Any(
                            productOrderLink =>
                                productOrderLink.Order!.User!.Name
                                    .ToLower()
                                    .Contains(
                                        request.CustomerName.ToLower()
                                    )
                        )
                );

            if (!string.IsNullOrWhiteSpace(request.CustomerSurname))
                predicate = predicate.And(
                    product =>
                        product.ProductOrders!.Any(
                            productOrderLink =>
                                productOrderLink.Order!.User!.Surname
                                    .ToLower()
                                    .Contains(
                                        request.CustomerSurname.ToLower()
                                    )
                        )
                );

            if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
                predicate = predicate.And(
                    product =>
                        product.ProductOrders!.Any(
                            productOrderLink =>
                                productOrderLink.Order!.User!.Email!
                                    .ToLower()
                                    .Contains(
                                        request.CustomerEmail.ToLower()
                                    )
                        )
                );

            #endregion

            dataTuple.recordCount = await query.Where(predicate).CountAsync(cancellationToken);

            if (dataTuple.recordCount == 0) throw new NotFoundException(request.ToString(), "Products");

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
                Data = dataTuple.result,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                RecordCount = dataTuple.recordCount
            }
        );
    }
}