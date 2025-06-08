using Domain.Entities;

namespace Application.Users.Commands.RegisterUser;

public class RegisterUserCommandResponseDto
{
	public string  Id       { get; set; } = string.Empty;
	public string? UserName { get; set; }

	private class Mapping : Profile
	{
		public Mapping()
		{
			CreateMap<User, RegisterUserCommandResponseDto>();
		}
	}
}