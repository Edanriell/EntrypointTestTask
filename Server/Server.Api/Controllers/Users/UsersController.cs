using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Users.DeleteUser;
using Server.Application.Users.GetClients;
using Server.Application.Users.GetLoggedInUser;
using Server.Application.Users.LoginUser;
using Server.Application.Users.RegisterUser;
using Server.Application.Users.UpdateUser;
using Server.Domain.Abstractions;
using Server.Infrastructure.Authorization;

namespace Server.Api.Controllers.Users;

[ApiController]
[ApiVersion(
    ApiVersions.V1
)]
[Route(
    "api/v{version:apiVersion}/users"
)]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender) { _sender = sender; }

    [HttpGet]
    public async Task<IActionResult> GetClients(CancellationToken cancellationToken = default)
    {
        var query = new GetClientsQuery();

        Result<IReadOnlyList<GetClientsResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet(
        "me"
    )]
    [HasPermission(
        Permissions.UsersRead
    )]
    public async Task<IActionResult> GetLoggedInUser(CancellationToken cancellationToken)
    {
        var query = new GetLoggedInUserQuery();

        Result<UserResponse> result = await _sender.Send(
            query,
            cancellationToken
        );

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost(
        "register"
    )]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.Gender,
            request.Country,
            request.City,
            request.ZipCode,
            request.Street,
            request.Password);
 
        Result<Guid> result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [AllowAnonymous]
    [HttpPut("update")]
    public async Task<IActionResult> Update(
        UpdateUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateUserCommand(
            request.UserId,
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.Gender,
            request.Country,
            request.City,
            request.ZipCode,
            request.Street);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [AllowAnonymous]
    [HttpDelete("delete/{userId:guid}")]
    public async Task<IActionResult> Delete(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(userId);

        Result result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [AllowAnonymous]
    [HttpPost(
        "login"
    )]
    public async Task<IActionResult> LogIn(
        LogInUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(
            request.Email,
            request.Password
        );

        Result<AccessTokenResponse> result = await _sender.Send(
            command,
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
    }
}
