namespace Application.Users.Queries.GetUser;

public class GetUserValidator : AbstractValidator<GetUserQuery>
{
	public GetUserValidator()
	{
		RuleFor(x => x.Id)
		   .NotEmpty()
		   .WithMessage("User id can't be empty.")
		   .Must(BeAValidGuid)
		   .WithMessage("User id must be a valid GUID");
	}

	private bool BeAValidGuid(string id)
	{
		return Guid.TryParse(id, out _);
	}
}