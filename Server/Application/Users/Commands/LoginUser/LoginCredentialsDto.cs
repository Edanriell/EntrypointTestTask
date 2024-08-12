namespace Application.Users.Commands.LoginUser;

public sealed class LoginCredentialsDto
{
	public string  UserName              { get; set; } = string.Empty;
	public string  Password              { get; set; } = string.Empty;
	public string? TwoFactorCode         { get; set; }
	public string? TwoFactorRecoveryCode { get; set; }
}