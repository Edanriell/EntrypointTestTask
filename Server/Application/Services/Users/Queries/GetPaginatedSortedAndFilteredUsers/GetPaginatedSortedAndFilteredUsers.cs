using Application.Services.Users.Dto;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Users.Queries.GetPaginatedSortedAndFilteredUsers;

public record GetPaginatedSortedAndFilteredUsersQuery : IRequest<IResult>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public DateTime? OrderCreatedAt { get; set; }
    public DateTime? OrderUpdatedAt { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public string? OrderProductCode { get; set; }
    public string? OrderProductName { get; set; }
}

public class GetPaginatedSortedAndFilteredUsersQueryHandler(
    UserManager<User> userManager,
    IMapper mapper,
    IMemoryCache memoryCache) : IRequestHandler<GetPaginatedSortedAndFilteredUsersQuery, IResult>
{
    public async Task<IResult> Handle(GetPaginatedSortedAndFilteredUsersQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

        if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, UserDto[] result) dataTuple))
        {
            var query = userManager.Users
                .AsNoTracking()
                .Include(user => user.Orders)!
                .ThenInclude(order => order.OrderProducts)!
                .ThenInclude(productOrderLink => productOrderLink.Product)
                .AsSplitQuery();

            var predicate = PredicateBuilder.New<User>(true);

            #region Custom Users Filters

            if (!string.IsNullOrWhiteSpace(request.Name))
                predicate = predicate.And(
                    user => user.Name.ToLower() == request.Name!.ToLower()
                );

            if (!string.IsNullOrWhiteSpace(request.Surname))
                predicate = predicate.And(
                    user =>
                        user.Surname.ToLower() == request.Surname!.ToLower()
                );

            if (!string.IsNullOrWhiteSpace(request.Email))
                predicate = predicate.And(
                    user =>
                        user.Email!.ToLower() == request.Email!.ToLower()
                );

            if (!string.IsNullOrWhiteSpace(request.UserName))
                predicate = predicate.And(
                    user =>
                        user.UserName!.ToLower()
                        == request.UserName!.ToLower()
                );

            if (request.OrderCreatedAt is not null)
                predicate = predicate.And(
                    user =>
                        user.Orders!.Any(
                            order => order.CreatedAt == request.OrderCreatedAt
                        )
                );

            if (request.OrderUpdatedAt is not null)
                predicate = predicate.And(
                    user =>
                        user.Orders!.Any(
                            order => order.UpdatedAt == request.OrderUpdatedAt
                        )
                );

            if (request.OrderStatus >= 0)
                predicate = predicate.And(
                    user =>
                        user.Orders!.Any(
                            order => order.Status == request.OrderStatus
                        )
                );

            if (!string.IsNullOrWhiteSpace(request.OrderProductCode))
                predicate = predicate.And(
                    user =>
                        user.Orders!.Any(
                            order =>
                                order.OrderProducts!.Any(
                                    productOrderLink =>
                                        productOrderLink.Product!.Code
                                            .ToLower()
                                            .Contains(
                                                request.OrderProductCode.ToLower()
                                            )
                                )
                        )
                );

            if (!string.IsNullOrWhiteSpace(request.OrderProductName))
                predicate = predicate.And(
                    user =>
                        user.Orders!.Any(
                            order =>
                                order.OrderProducts!.Any(
                                    productOrderLink =>
                                        productOrderLink.Product!.ProductName
                                            .ToLower()
                                            .Contains(
                                                request.OrderProductName.ToLower()
                                            )
                                )
                        )
                );

            #endregion

            dataTuple.recordCount = await query.Where(predicate).CountAsync(cancellationToken);

            if (dataTuple.recordCount == 0) throw new NotFoundException(request.ToString(), "Users");

            query = query
                .Where(predicate)
                .OrderBy($"{request.SortColumn} {request.SortOrder}")
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize);

            dataTuple.result =
                await query.ProjectTo<UserDto>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

            memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
        }

        return TypedResults.Ok(
            new GetPaginatedSortedAndFilteredUsersQueryResponseDto<UserDto[]>
            {
                Data = dataTuple.result,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                RecordCount = dataTuple.recordCount
            }
        );
    }
}