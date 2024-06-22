namespace Application.Services.Users.Queries.GetPaginatedSortedAndFilteredUsers;

public class GetPaginatedSortedAndFilteredUsersQueryResponseDto<T>
{
    public T Data { get; set; } = default!;
    public int? RecordCount { get; set; }
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
}