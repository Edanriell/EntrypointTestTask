namespace Application.Users.Queries.GetUser;

public class GetUserQueryResponseDto<T>
{
	public T Data { get; set; } = default!;
}