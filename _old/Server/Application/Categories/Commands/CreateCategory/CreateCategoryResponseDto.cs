using Domain.Entities;

namespace Application.Categories.Commands.CreateCategory;

public class CreateCategoryResponseDto
{
	public int    Id          { get; set; }
	public string Name        { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;

	private class Mapping : Profile
	{
		public Mapping()
		{
			CreateMap<Category, CreateCategoryResponseDto>();
		}
	}
}