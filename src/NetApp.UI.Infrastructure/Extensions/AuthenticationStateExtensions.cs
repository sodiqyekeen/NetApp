using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace NetApp.UI.Infrastructure.Extensions;
public static class AuthenticationStateExtensions
{
    public static AuthenticationState Anonymous { get; set; } = new(new ClaimsPrincipal(new ClaimsPrincipal()));

    public static string? Username(this AuthenticationState authenticationState) =>
        authenticationState.User.FindFirst("username")?.Value;

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

    public static bool Authorize(this ClaimsPrincipal currentUser, string permission)
    {
        var permissions = currentUser.FindAll(c => c.Type == Application.ApplicationConstants.CustomClaimTypes.Permission);
        return permissions != null && permissions.Any(r => r.Value.Contains(permission));
    }
}
