using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace NetApp.UI.Infrastructure.Extensions;
public static class AuthenticationStateExtensions
{
    public static AuthenticationState Anonymous { get; set; } = new(new ClaimsPrincipal(new ClaimsPrincipal()));

    public static string Username(this AuthenticationState authenticationState) =>
        authenticationState.User.FindFirst(ClaimTypes.Name)?.Value ?? "";

    public static string UserEmail(this AuthenticationState authenticationState) =>
        authenticationState.User.FindFirst(ClaimTypes.Email)?.Value ?? "";

    public static string UserId(this AuthenticationState authenticationState) =>
        authenticationState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

    public static bool IsAnonymous(this AuthenticationState authenticationState) =>
       authenticationState == Anonymous;

    public static bool IsTokenExpired(this AuthenticationState currentAuthState)
    {
        TimeSpan timeDiff = GetTokenExpiration(currentAuthState);
        return timeDiff.Seconds < 10;
    }

    public static bool ShouldRefreshToken(this AuthenticationState currentAuthState)
    {
        TimeSpan timeDiff = GetTokenExpiration(currentAuthState);
        return timeDiff.Minutes <= 5;
    }

    public static TimeSpan GetTokenExpiration(this AuthenticationState currentAuthState)
    {
        var expiry = currentAuthState.User.FindFirst(c => c.Type.Equals("exp"))!.Value;
        var expireTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(expiry));
        return expireTime - DateTime.UtcNow;
    }

    public static bool Authorize(this ClaimsPrincipal currentUser, string permission) =>
     currentUser.FindFirst(c => c.Type == SharedConstants.CustomClaimTypes.Permission && c.Value.Contains(permission)) != null;

    public static bool Authorize(this AuthenticationState authState, string permission) =>
    authState.User.FindFirst(c => c.Type == SharedConstants.CustomClaimTypes.Permission && c.Value.Contains(permission)) != null;
}
