using Application.Users.Dto;
using Domain.Entities;

namespace Application.Users.Queries.GetAllUsers;

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
						   .AsSplitQuery()
						   .Include(user => user.Orders)!
						   .ThenInclude(order => order.OrderProducts)!
						   .ThenInclude(productOrderLink => productOrderLink.Product)
						   .ToListAsync(cancellationToken);


			dataTuple.recordCount = users.Count;

			if (dataTuple.recordCount == 0)
				return TypedResults.NotFound(new { Message = "No users found." });

			dataTuple.result = mapper.Map<UserDto[]>(users);

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 2, 0));
		}

		return TypedResults.Ok(
				new GetAllUsersQueryResponseDto<UserDto[]>
				{
					Data        = dataTuple.result,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}