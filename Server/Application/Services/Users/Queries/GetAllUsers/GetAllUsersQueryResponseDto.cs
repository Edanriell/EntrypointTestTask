namespace Application.Services.Users.Queries.GetAllUsers;

public class GetAllUsersQueryResponseDto<T>
{
    public T Data { get; set; } = default!;
    public int? RecordCount { get; set; }
}