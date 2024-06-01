using Application.Services.Users.Commands.ChangeUserRole;
using Application.Services.Users.Commands.ConfirmUserEmail;
using Application.Services.Users.Commands.DeleteUser;
using Application.Services.Users.Commands.ForgotPassword;
using Application.Services.Users.Commands.LoginUser;
using Application.Services.Users.Commands.LoginUserForAdminPanel;
using Application.Services.Users.Commands.LogoutUser;
using Application.Services.Users.Commands.RefreshToken;
using Application.Services.Users.Commands.RegisterUser;
using Application.Services.Users.Commands.ResendConfirmationEmail;
using Application.Services.Users.Commands.ResetUserPassword;
using Application.Services.Users.Commands.TwoFactorAuthentication;
using Application.Services.Users.Commands.UpdateUserInfo;
using Application.Services.Users.Queries.IsUserInPolicy;
using Application.Services.Users.Queries.IsUserInRole;
using Application.Services.Users.Queries.UserInfo;
using Domain.Constants;
using Web.Infrastructure;

namespace Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup(this);

        routeGroupBuilder
            .MapPost(RegisterUser, "register")
            .MapPost(LoginUser, "login")
            .MapGet(LogoutUser, "logout")
            .MapPost(RefreshToken, "refresh")
            .MapGet(ConfirmUserEmail, "confirmEmail")
            .MapPost(ResendConfirmationEmail, "resendConfirmationEmail")
            .MapPost(ForgotPassword, "forgotPassword")
            .MapPost(ResetUserPassword, "resetPassword")
            .MapPost(TwoFactorAuthentication, "/2fa")
            .MapGet(UserInfo, "userInfo")
            .MapPost(UpdateUserInfo, "updateUserInfo")
            .MapDelete(DeleteUser, "delete")
            .MapGet(IsUserInRole, "isInRole")
            .MapGet(IsUserInPolicy, "isInPolicy")
            .MapGet(ChangeUserRole, "changeUserRole");
    }

    private Task<IResult> RegisterUser(ISender sender,
        [FromBody] RegisterUserCommand command)
    {
        return sender.Send(command);
    }

    private Task<IResult> LoginUser(ISender sender,
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

    private Task<IResult> LoginUserForAdminPanel(ISender sender,
        [AsParameters] LoginUserForAdminPanelCommand command)
    {
        return sender.Send(command);
    }
}