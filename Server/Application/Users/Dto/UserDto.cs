using Domain.Entities;
using Domain.Enums;

namespace Application.Users.Dto;

public class UserDto
{
	public string                 Id        { get; set; } = null!;
	public string                 Username  { get; set; } = null!;
	public string                 Email     { get; set; } = null!;
	public string                 Name      { get; set; } = null!;
	public string                 Surname   { get; set; } = null!;
	public string                 Address   { get; set; } = null!;
	public DateTime               BirthDate { get; set; }
	public Gender                 Gender    { get; set; }
	public DateTime               CreatedAt { get; set; }
	public DateTime?              LastLogin { get; set; }
	public byte[]?                Photo     { get; set; }
	public ICollection<OrderDto>? Orders    { get; set; }

	private class Mapping : Profile
	{
		// public Mapping()
		// {
		//     CreateMap<User, UserDto>();
		// }

		public Mapping()
		{
			CreateMap<User, UserDto>()
			   .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));
		}
	}
}