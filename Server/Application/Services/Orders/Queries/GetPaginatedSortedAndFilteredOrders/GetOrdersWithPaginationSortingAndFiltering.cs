using Application.Interfaces;
using Application.Services.Orders.Dto;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Orders.Queries.GetPaginatedSortedAndFilteredOrders;

public record GetPaginatedSortedAndFilteredOrdersQuery
    : IRequest<IResult>
{
    public int PageIndex { get; init; } = 0;
    public int PageSize { get; init; } = 1;
    public string SortColumn { get; init; } = "Id";
    public string SortOrder { get; init; } = "DESC";
    public OrderStatus? OrderStatus { get; set; }
    public string? OrderShipAddress { get; set; }
    public string? OrderOrdererUserName { get; set; }
    public string? OrderOrdererUserSurname { get; set; }
    public string? OrderOrdererUserEmail { get; set; }
    public string? OrderProductName { get; set; }
}

public class GetPaginatedSortedAndFilteredOrdersQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    IMemoryCache memoryCache)
    : IRequestHandler<GetPaginatedSortedAndFilteredOrdersQuery, IResult>
{
    public async Task<IResult> Handle(
        GetPaginatedSortedAndFilteredOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        if (context.Orders is null) return TypedResults.NotFound("No orders has been found");

        var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

        if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, OrderDto[] result) dataTuple))
        {
            var query = context.Orders
                .AsNoTracking()
                .Include(order => order.User)
                .Include(order => order.OrderProducts)!
                .ThenInclude(productOrderLink => productOrderLink.Product)
                .AsSplitQuery();

            var predicate = PredicateBuilder.New<Order>(true);

            #region Custom Orders Filters

            if (request.OrderStatus is not null)
                predicate = predicate.And(
                    order => order.Status == request.OrderStatus.Value
                );

            if (!string.IsNullOrWhiteSpace(request.OrderShipAddress))
                predicate = predicate.And(
                    order => order.ShipAddress.ToLower().Contains(request.OrderShipAddress.ToLower())
                );

            if (!string.IsNullOrWhiteSpace(request.OrderOrdererUserName))
                predicate = predicate.And(
                    order =>
                        order.User!.Name
                            .ToLower()
                            .Contains(request.OrderOrdererUserName.ToLower())
                );

            if (!string.IsNullOrWhiteSpace(request.OrderOrdererUserSurname))
                predicate = predicate.And(
                    order =>
                        order.User!.Surname
                            .ToLower()
                            .Contains(request.OrderOrdererUserSurname.ToLower())
                );

            if (!string.IsNullOrWhiteSpace(request.OrderOrdererUserEmail))
                predicate = predicate.And(
                    order =>
                        order.User!.Email!
                            .ToLower()
                            .Contains(request.OrderOrdererUserEmail.ToLower())
                );

            if (!string.IsNullOrWhiteSpace(request.OrderProductName))
                predicate = predicate.And(
                    order =>
                        order.OrderProducts!.Any(
                            productOrderLink =>
                                productOrderLink.Product!.ProductName
                                    .ToLower()
                                    .Contains(request.OrderProductName.ToLower())
                        )
                );

            #endregion

            dataTuple.recordCount = await query.Where(predicate).CountAsync(cancellationToken);

            if (dataTuple.recordCount == 0) throw new NotFoundException(request.ToString(), "Orders");

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
                Data = dataTuple.result,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                RecordCount = dataTuple.recordCount
            }
        );

        // Changed AsQueryable() to AsSplitQuery Maybe performance will be a bit better
        // Need to read about this and rewrite
    }
}