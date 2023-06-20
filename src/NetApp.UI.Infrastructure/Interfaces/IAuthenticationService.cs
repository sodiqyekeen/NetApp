using System.Security.Claims;
using NetApp.Application.Dtos.Identity;

namespace NetApp.UI.Infrastructure.Services;
public interface IAuthenticationService
{
    Task<IResponse> LoginAsync(AuthenticationRequest request);
    Task LogoutAsync();
    Task<ClaimsPrincipal> GetCurrentUser();
    Task TryRefreshToken(bool force = false);
}
