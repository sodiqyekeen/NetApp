using System.Security.Claims;

namespace NetApp.UI.Infrastructure.Services;
public interface IAuthenticationService
{
    Task<IResponse> LoginAsync(AuthenticationRequest request);
    Task LogoutAsync();
    Task<ClaimsPrincipal> GetCurrentUser();
    Task TryRefreshToken(bool force = false);
}
