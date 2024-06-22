namespace Application.Services.Users.Queries.UserInfo;

public class UserInfoDto
{
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public byte[] Photo { get; set; } = null!;
    public IList<string> Roles { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsEmailConfirmed { get; set; }
}