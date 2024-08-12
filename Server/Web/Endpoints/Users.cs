using Application.Users.Commands.ChangeUserRole;
using Application.Users.Commands.ConfirmUserEmail;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.ForgotPassword;
using Application.Users.Commands.LoginUser;
using Application.Users.Commands.LogoutUser;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.RegisterUser;
using Application.Users.Commands.ResendConfirmationEmail;
using Application.Users.Commands.ResetUserPassword;
using Application.Users.Commands.TwoFactorAuthentication;
using Application.Users.Commands.UpdateUser;
using Application.Users.Commands.UpdateUserInfo;
using Application.Users.Queries.GetAllUsers;
using Application.Users.Queries.GetPaginatedSortedAndFilteredUsers;
using Application.Users.Queries.GetUser;
using Application.Users.Queries.IsUserInPolicy;
using Application.Users.Queries.IsUserInRole;
using Application.Users.Queries.UserInfo;
using Domain.Constants;
using Web.Infrastructure;

namespace Web.Endpoints;

public class Users : EndpointGroupBase
{
	public override void Map(WebApplication app)
	{
		var routeGroupBuilder = app.MapGroup(this);

		routeGroupBuilder
		   .MapGet(GetAllUsers, "All")
		   .MapGet(GetUser,     "{id}")
		   .MapGet(GetPaginatedSortedAndFilteredUsers)
		   .MapPost(RegisterUser, "Register")
		   .MapPost(LoginUser,    "Login")
		   .MapGet(LogoutUser, "logout")
		   .MapPost(RefreshToken, "Refresh")
		   .MapGet(ConfirmUserEmail, "ConfirmEmail")
		   .MapPost(ResendConfirmationEmail, "ResendConfirmationEmail")
		   .MapPost(ForgotPassword,          "ForgotPassword")
		   .MapPost(ResetUserPassword,       "ResetPassword")
		   .MapPost(TwoFactorAuthentication, "/2fa")
		   .MapGet(UserInfo, "UserInfo")
		   .MapPost(UpdateUserInfo, "UpdateUserInfo")
		   .MapPut(UpdateUser, "Update")
		   .MapDelete(DeleteUser, "Delete")
		   .MapGet(IsUserInRole,   "IsInRole")
		   .MapGet(IsUserInPolicy, "IsInPolicy")
		   .MapGet(ChangeUserRole, "ChangeUserRole");
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> GetAllUsers(ISender sender, [AsParameters] GetAllUsersQuery query)
	{
		return sender.Send(query);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> GetUser(ISender sender, [AsParameters] GetUserQuery query)
	{
		return sender.Send(query);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> GetPaginatedSortedAndFilteredUsers(ISender sender,
															 [AsParameters]
															 GetPaginatedSortedAndFilteredUsersQuery query)
	{
		return sender.Send(query);
	}

	private Task<IResult> RegisterUser(ISender                        sender,
									   [FromBody] RegisterUserCommand command)
	{
		return sender.Send(command);
	}

	private Task<IResult> LoginUser(ISender                         sender,
									[AsParameters] LoginUserCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> LogoutUser(ISender sender, [AsParameters] LogoutUserCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> RefreshToken(ISender sender, [FromBody] RefreshTokenCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> ConfirmUserEmail(ISender sender, [AsParameters] ConfirmUserEmailCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> ResendConfirmationEmail(ISender sender, [FromBody] ResendConfirmationEmailCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> ForgotPassword(ISender sender, [FromBody] ForgotPasswordCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> ResetUserPassword(ISender sender, [FromBody] ResetUserPasswordCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> TwoFactorAuthentication(ISender sender, [FromBody] TwoFactorAuthenticationCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> UserInfo(ISender sender, [AsParameters] UserInfoQuery query)
	{
		return sender.Send(query);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> UpdateUserInfo(ISender sender, [FromBody] UpdateUserInfoCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> UpdateUser(ISender sender, [FromBody] UpdateUserCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUserAccount)]
	private Task<IResult> DeleteUser(ISender sender, [AsParameters] DeleteUserCommand command)
	{
		return sender.Send(command);
	}

	[Authorize(Policy = Policies.CanManageUsers)]
	private Task<IResult> IsUserInRole(ISender sender, [AsParameters] IsUserInRoleQuery query)
	{
		return sender.Send(query);
	}

	[Authorize(Policy = Policies.CanManageUsers)]
	private Task<IResult> IsUserInPolicy(ISender sender, [AsParameters] IsUserInPolicyQuery query)
	{
		return sender.Send(query);
	}

	[Authorize(Policy = Policies.CanChangeUserRole)]
	private Task<IResult> ChangeUserRole(ISender sender, [AsParameters] ChangeUserRoleCommand command)
	{
		return sender.Send(command);
	}
}