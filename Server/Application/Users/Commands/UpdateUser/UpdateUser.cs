using Domain.Entities;

namespace Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand : IRequest<IResult>
{
	public string    Id        { get; set; } = string.Empty;
	public string?   Name      { get; set; }
	public string?   Surname   { get; set; }
	public string?   Username  { get; set; }
	public string?   Address   { get; set; }
	public DateTime? BirthDate { get; set; }
	public string?   Photo     { get; set; }
}

public class UpdateUserCommandHandler(UserManager<User> userManager) : IRequestHandler<UpdateUserCommand, IResult>
{
	public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
	{
		var user = await userManager.FindByIdAsync(request.Id);

		if (user is null)
			return TypedResults.NotFound($"User with ID {request.Id} was not found.");

		if (!string.IsNullOrWhiteSpace(request.Name))
			user.Name = request.Name;
		if (!string.IsNullOrWhiteSpace(request.Surname))
			user.Surname = request.Surname;
		if (!string.IsNullOrWhiteSpace(request.Username))
			user.UserName = request.Username;
		if (!string.IsNullOrWhiteSpace(request.Address))
			user.Address = request.Address;
		if (request.BirthDate.HasValue)
			user.BirthDate = request.BirthDate.Value;
		if (!string.IsNullOrWhiteSpace(request.Photo))
			user.Photo = await ConvertBase64ToByteArray(request.Photo);

		var result = await userManager.UpdateAsync(user);

		if (!result.Succeeded)
			return TypedResults.BadRequest($"User with ID {user.Id} could not be updated.");

		return TypedResults.Ok($"User with ID {user.Id} successfully updated.");
	}

	private async Task<byte[]> ConvertBase64ToByteArray(string base64String)
	{
		const int maxBase64Length = 1398368;
		if (base64String.Length > maxBase64Length)
			throw new ArgumentException("The provided image is too large. The maximum allowed size is 1MB.");
		return await Task.FromResult(Convert.FromBase64String(base64String));
	}
}