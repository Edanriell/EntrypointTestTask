using Application.Services.Users.Dto;
using Domain.Entities;

namespace Application.Services.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<IResult>;

public class GetAllUsersQueryHandler(UserManager<User> userManager, IMapper mapper, IMemoryCache memoryCache)
    : IRequestHandler<GetAllUsersQuery, IResult>
{
    public async Task<IResult> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

        if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, UserDto[] result) dataTuple))
        {
            var users = await userManager.Users
                .AsNoTracking()
                .Include(user => user.Orders)!
                .ThenInclude(order => order.OrderProducts)!
                .ThenInclude(productOrderLink => productOrderLink.Product)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);


            dataTuple.recordCount = users.Count;

            if (dataTuple.recordCount == 0) throw new NotFoundException(request.ToString(), "Users");

            dataTuple.result = mapper.Map<UserDto[]>(users);

            memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
        }

        return TypedResults.Ok(
            new GetAllUsersQueryResponseDto<UserDto[]>
            {
                Data = dataTuple.result,
                RecordCount = dataTuple.recordCount
            }
        );
    }
}