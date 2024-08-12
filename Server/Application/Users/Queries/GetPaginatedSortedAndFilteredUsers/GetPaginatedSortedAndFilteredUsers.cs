using Application.Users.Dto;
using Domain.Entities;
using Domain.Enums;

namespace Application.Users.Queries.GetPaginatedSortedAndFilteredUsers;

public record GetPaginatedSortedAndFilteredUsersQuery : IRequest<IResult>
{
	public int       PageIndex         { get; set; } = 0;
	public int       PageSize          { get; set; } = 10;
	public string?   SortColumn        { get; set; } = "Id";
	public string?   SortOrder         { get; set; } = "DESC";
	public string?   UserName          { get; set; }
	public string?   Name              { get; set; }
	public string?   Surname           { get; set; }
	public string?   Email             { get; set; }
	public DateTime? BirthDate         { get; set; }
	public Gender?   Gender            { get; set; }
	public DateTime? CreatedAt         { get; set; }
	public string?   Address           { get; set; }
	public DateTime? LastLogin         { get; set; }
	public string?   Role              { get; set; }
	public DateTime? RegistrationStart { get; set; }
	public DateTime? RegistrationEnd   { get; set; }
	public int?      AgeFrom           { get; set; }
	public int?      AgeTo             { get; set; }
}

public class GetPaginatedSortedAndFilteredUsersQueryHandler(
	UserManager<User> userManager,
	IMapper           mapper,
	IMemoryCache      memoryCache) : IRequestHandler<GetPaginatedSortedAndFilteredUsersQuery, IResult>
{
	public async Task<IResult> Handle(GetPaginatedSortedAndFilteredUsersQuery request,
									  CancellationToken                       cancellationToken)
	{
		var cacheKey = $"{request.GetType()}-{JsonSerializer.Serialize(request)}";

		if (!memoryCache.TryGetValue(cacheKey, out (int recordCount, UserDto[] result) dataTuple))
		{
			var query = userManager.Users
			   .AsNoTracking();

			var predicate = PredicateBuilder.New<User>(true);

			#region Custom Users Filters

			if (!string.IsNullOrWhiteSpace(request.Name))
				predicate = predicate.And(user => user.Name.ToLower().Contains(request.Name.ToLower()));

			if (!string.IsNullOrWhiteSpace(request.Surname))
				predicate = predicate.And(user => user.Surname.ToLower().Contains(request.Surname.ToLower()));

			if (!string.IsNullOrWhiteSpace(request.Email))
				predicate = predicate.And(user => user.Email!.ToLower().Contains(request.Email.ToLower()));

			if (!string.IsNullOrWhiteSpace(request.UserName))
				predicate = predicate.And(user => user.UserName!.ToLower().Contains(request.UserName.ToLower()));

			if (request.BirthDate.HasValue)
				predicate = predicate.And(user => user.BirthDate.Date == request.BirthDate.Value.Date);

			if (request.Gender.HasValue)
				predicate = predicate.And(user => user.Gender == request.Gender);

			if (request.CreatedAt.HasValue)
				predicate = predicate.And(user => user.CreatedAt.Date == request.CreatedAt.Value.Date);

			if (!string.IsNullOrWhiteSpace(request.Address))
				predicate = predicate.And(user => user.Address.ToLower().Contains(request.Address.ToLower()));

			if (request.LastLogin.HasValue)
				predicate = predicate.And(user =>
					user.LastLogin.HasValue && user.LastLogin.Value.Date == request.LastLogin.Value.Date);

			if (!string.IsNullOrWhiteSpace(request.Role))
			{
				var rolePredicate                               = PredicateBuilder.New<User>(false);
				var usersInRole                                 = await userManager.GetUsersInRoleAsync(request.Role);
				foreach (var user in usersInRole) rolePredicate = rolePredicate.Or(u => u.Id == user.Id);
				predicate = predicate.And(rolePredicate);
			}

			if (request.RegistrationStart.HasValue)
				predicate = predicate.And(user => user.CreatedAt >= request.RegistrationStart.Value);

			if (request.RegistrationEnd.HasValue)
				predicate = predicate.And(user => user.CreatedAt <= request.RegistrationEnd.Value);

			if (request.AgeFrom.HasValue)
			{
				var minDate = DateTime.Now.AddYears(-request.AgeFrom.Value);
				predicate = predicate.And(user => user.BirthDate <= minDate);
			}

			if (request.AgeTo.HasValue)
			{
				var maxDate = DateTime.Now.AddYears(-request.AgeTo.Value);
				predicate = predicate.And(user => user.BirthDate >= maxDate);
			}

			#endregion

			dataTuple.recordCount = await query.Where(predicate).CountAsync(cancellationToken);

			if (dataTuple.recordCount == 0)
				return TypedResults.NotFound(new { Message = "No users found matching the criteria." });

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
					Data        = dataTuple.result,
					PageIndex   = request.PageIndex,
					PageSize    = request.PageSize,
					RecordCount = dataTuple.recordCount
				}
			);
	}
}