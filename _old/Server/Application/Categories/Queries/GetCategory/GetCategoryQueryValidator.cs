namespace Application.Categories.Queries.GetCategory;

public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
{
	public GetCategoryQueryValidator()
	{
		RuleFor(x => x.Id)
		   .NotEmpty()
		   .WithMessage("Category ID is required.")
		   .GreaterThan(0)
		   .WithMessage("Category ID must be greater than 0.");
	}
}