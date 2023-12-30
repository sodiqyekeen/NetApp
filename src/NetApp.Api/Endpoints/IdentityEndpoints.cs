using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetApp.Shared.Constants;

namespace NetApp.Api.Endpoints;
internal static class IdentityEndpoints
{
    public static RouteGroupBuilder MapIdentityEndpoints(this RouteGroupBuilder group)
    {
        group = group.MapGroup("identity");
        group.MapPost("/login", async (AuthenticationRequest request, HttpContext context, IIdentityService identityService) =>
             Results.Ok(await identityService.LoginAsync(request, GetIPAddress(context))))
             .Produces<IResponse<AuthenticationResponse>>(StatusCodes.Status200OK);

        group.MapPost("/refresh-token", async (RefreshTokenRequest request, HttpContext context, IIdentityService identityService) =>
             Results.Ok(await identityService.RefreshTokenAsync(request, GetIPAddress(context))))
             .Produces<IResponse<AuthenticationResponse>>(StatusCodes.Status200OK);

        group.MapGet("/users", [Authorize] async (IIdentityService identityService, [AsParameters] PagingOptions pagingOptions, CancellationToken cancellationToken) =>
            Results.Ok(await identityService.GetUsersAsync(pagingOptions, cancellationToken)))
            .Produces<IResponse<IEnumerable<UserDto>>>(StatusCodes.Status200OK);

        group.MapGet("/users/{id}", [Authorize(Policy = Permissions.User.View)] async (IIdentityService identityService, string id) =>
            Results.Ok(await identityService.GetUser(id)))
                .Produces<IResponse<UserDto>>(StatusCodes.Status200OK);

        group.MapPut("/users/{id}", [Authorize] async (EditUserRequest request, string id, IIdentityService identityService) =>
            Results.Ok(await identityService.UpdateUserAsync(id, request)))
            .Produces<IResponse>(StatusCodes.Status200OK);

        group.MapPost("/users", [Authorize] async (RegisterRequest request, string origin, IIdentityService identityService) =>
            Results.Ok(await identityService.RegisterAsync(request, origin)))
            .Produces<IResponse<string>>(StatusCodes.Status200OK);

        group.MapPost("/users/{userId}/confirm-email", async (string userId, [FromQuery] string code, IIdentityService identityService) =>
            Results.Ok(await identityService.ConfirmEmailAsync(userId, code)))
            .Produces<IResponse<string>>(StatusCodes.Status200OK);

        group.MapPost("/users/{userId}/resend-confirmation-email", [Authorize] async (ConfirmationMailRequest request, string userId, HttpContext context, IIdentityService identityService) =>
        {
            await identityService.ResendConfirmationMailAsync(request, GetIPAddress(context));
            return Results.NoContent();
        })
          .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/users/forgot-password", async (ForgotPasswordRequest request, IIdentityService identityService) =>
        {
            await identityService.ForgotPasswordAsync(request);
            return Results.NoContent();
        }).Produces(StatusCodes.Status204NoContent);

        group.MapPost("/users/{userId}/reset-password", async (ResetPasswordRequest request, string userId, IIdentityService identityService) =>
             Results.Ok(await identityService.ResetPasswordAsync(request)))
             .Produces<IResponse>(StatusCodes.Status200OK);

        group.MapPut("/users/{userId}/update-password", [Authorize] async (UpdatePasswordRequest request, string userId, IIdentityService identityService) =>
             Results.Ok(await identityService.UpdatePasswordAsync(request)))
             .Produces<IResponse>(StatusCodes.Status200OK);

        group.MapDelete("/users/{userId}", [Authorize] async (string userId, IIdentityService identityService) =>
            Results.Ok(await identityService.DeleteUserAsync(userId)))
            .Produces<IResponse>(StatusCodes.Status200OK);

        group.MapGet("users/{userId}/roles", [Authorize] async (IIdentityService identityService, string userId, CancellationToken cancellationToken) =>
            Results.Ok(await identityService.GetRolesAsync(userId)))
            .Produces<IResponse<UserRolesResponse>>(StatusCodes.Status200OK);

        group.MapPut("users/{userId}/roles", [Authorize] async (UpdateUserRoleRequest request, string userId, IIdentityService identityService) =>
            Results.Ok(await identityService.UpdateUserRoleAsync(userId, request)))
            .Produces<IResponse>(StatusCodes.Status200OK);

        return group;
    }

    static string GetIPAddress(HttpContext httpContext) =>
                  httpContext.Request.Headers.ContainsKey("X-Forwarded-For")
                      ? httpContext.Request.Headers["X-Forwarded-For"].ToString()
                      : httpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() ?? "";
}
