using Application.Users.Dto;
using Domain.Entities;

namespace Application.Users.Queries.GetUser;

public record GetUserQuery : IRequest<IResult>
{
	public string Id { get; set; }
}

public class GetUserQueryHandler(UserManager<User> userManager, IMapper mapper, IMemoryCache memoryCache)
	: IRequestHandler<GetUserQuery, IResult>
{
	public async Task<IResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, UserDto? result) dataTuple))
		{
			var entity = await userManager.Users
							.AsNoTracking()
							.Include(user => user.Orders)!
							.ThenInclude(order => order.OrderProducts)!
							.ThenInclude(productOrderLink => productOrderLink.Product)
							.Where(user => user.Id == request.Id)
							.ProjectTo<UserDto>(mapper.ConfigurationProvider)
							.AsSplitQuery()
							.FirstOrDefaultAsync(cancellationToken);


			if (entity is null)
			{
				dataTuple.recordCount = 0;
			}
			else
			{
				dataTuple.recordCount = 1;
				dataTuple.result      = entity;
			}

			memoryCache.Set(cacheKey, dataTuple, new TimeSpan(0, 1, 0));
		}

		Guard.Against.NotFound(request.Id, dataTuple.result);

		return TypedResults.Ok(
				new GetUserQueryResponseDto<UserDto>
				{
					Data = dataTuple.result
				}
			);
	}
}