namespace Application.Services.Users.Commands.LoginUser;

public class LoginCredentialsDto
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? TwoFactorCode { get; set; }
    public string? TwoFactorRecoveryCode { get; set; }
}