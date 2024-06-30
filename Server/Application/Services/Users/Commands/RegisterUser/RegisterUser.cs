using Application.Exceptions;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services.Users.Commands.RegisterUser;

public record RegisterUserCommand : IRequest<IResult>
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Address { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public string? Photo { get; set; }
}

public class RegisterUserCommandHandler(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IEmailSender<User> emailSender,
    LinkGenerator linkGenerator,
    IHttpContextAccessor httpContextAccessor
)
    : IRequestHandler<RegisterUserCommand, IResult>
{
    public async Task<IResult> Handle(RegisterUserCommand request
        , CancellationToken cancellationToken)
    {
        var newUser = new User
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            Password = request.Password,
            Address = request.Address,
            BirthDate = request.BirthDate,
            Gender = request.Gender,
            Photo = request.Photo is null ? [] : await ConvertBase64ToByteArray(request.Photo),
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded) return ValidationProblemHelper.CreateValidationProblem(result);

        await SendConfirmationEmailAsync(newUser, httpContextAccessor.HttpContext!, newUser.Email);

        if (!await roleManager.RoleExistsAsync(Roles.Customer))
            await roleManager.CreateAsync(new IdentityRole(Roles.Customer));

        await userManager.AddToRoleAsync(newUser, Roles.Customer);

        return Results.Ok(
            new RegisterUserCommandResponseDto
            {
                Id = newUser.Id,
                Username = newUser.UserName
            }
        );
    }

    private async Task<byte[]> ConvertIFormFileToByteArray(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }

    private async Task<byte[]> ConvertBase64ToByteArray(string base64String)
    {
        return await Task.FromResult(Convert.FromBase64String(base64String));
    }

    private async Task SendConfirmationEmailAsync(User user, HttpContext context, string email, bool isChange = false)
    {
        var code = isChange
            ? await userManager.GenerateChangeEmailTokenAsync(user, email)
            : await userManager.GenerateEmailConfirmationTokenAsync(user);

        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var userId = await userManager.GetUserIdAsync(user);
        var userIdEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId));

        var routeValues = new RouteValueDictionary
        {
            ["userId"] = userIdEncoded,
            ["code"] = code
        };

        // Console.WriteLine(userIdEncoded);
        // Console.WriteLine(code);

        var confirmEmailUrl =
            linkGenerator.GetUriByName(context, "ConfirmUserEmail", routeValues)
            ?? throw new NotSupportedException(
                "Could not find endpoint named 'ConfirmUserEmail'.");

        // Console.WriteLine(confirmEmailUrl);

        // Static endpoint is bad
        // Also i'm not sure if emailSender will work, it is not configured
        await emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
    }
}

// EXAMPLE OF REQUEST
// {
// "name": "Name",
// "surname": "Surname",
// "email": "user@gmail.com",
// "userName": "NameSurname7",
// "phoneNumber": "+37129551250",
// "password": "sqr##Z22g6#NAM",
// "address": "Daugavpils, Lol",
// "gender": 0,
// "birthDate": "2011-03-20"
// }