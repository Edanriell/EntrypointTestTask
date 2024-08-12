using Domain.Entities;

namespace Application.Users.Queries.UserInfo;

public class UserInfoQueryResponseDto
{
	public string        Id               { get; set; } = null!;
	public string        Username         { get; set; } = null!;
	public string        Name             { get; set; } = null!;
	public string        Surname          { get; set; } = null!;
	public byte[]        Photo            { get; set; } = null!;
	public IList<string> Roles            { get; set; } = null!;
	public string        Email            { get; set; } = null!;
	public bool          IsEmailConfirmed { get; set; }

	private class Mapping : Profile
	{
		public Mapping()
		{
			CreateMap<User, UserInfoQueryResponseDto>()
			   .ForMember(dest => dest.Roles,            opt => opt.Ignore())
			   .ForMember(dest => dest.Email,            opt => opt.Ignore())
			   .ForMember(dest => dest.IsEmailConfirmed, opt => opt.Ignore());
		}
	}
}