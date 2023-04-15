using NetApp.Application.Dtos.Identity;
using NetApp.Domain.Models;

namespace NetApp.Application.Interfaces.Identity;

public interface IIdentityService
{
    Task<IResponse<AuthenticationResponse>> LoginAsync(AuthenticationRequest request, string ipAddress);
    Task<IResponse<AuthenticationResponse>> GetRefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
    Task<IResponse<string>> RegisterAsync(RegisterRequest request, string origin);
    Task<IResponse> UpdateUserAsync(string id, EditUserRequest request);
    Task<IResponse<IEnumerable<User>>> GetUsersAsync();
    Task<IResponse<string>> ConfirmEmailAsync(string userId, string code);
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<IResponse> ResetPasswordAsync(ResetPasswordRequest request);
    Task<IResponse> UpdatePasswordAsync(UpdatePasswordRequest request);
    Task<IResponse> DeleteUserAsync(string userId);
    Task ResendConfirmationMailAsync(ConfirmationMailRequest request, string ipAddress);
    Task<IResponse<UserRolesResponse>> GetRolesAsync(string userId);
    Task<IResponse> UpdateUserRoleAsync(string id, UpdateUserRoleRequest request);
}
