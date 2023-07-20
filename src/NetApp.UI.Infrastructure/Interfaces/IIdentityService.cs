namespace NetApp.UI.Infrastructure;
public interface IIdentityService
{
    Task<PaginatedResponse<User>> GetUsersAsync(TableState state, string? searchString, CancellationToken cancellationToken);
    Task<IResponse> UpdateUserAsync(string id, EditUserRequest request);
    Task<IResponse<string>> RegisterUserAsync(RegisterRequest request);
    Task<IResponse> DeleteUserAsync(string userId);
    Task<IResponse> ConfirmEmailAsync(string userId, string code);
    Task<IResponse> ResendConfirmationMailAsync(ConfirmationMailRequest request);
    Task<IResponse<UserRolesResponse>> GetRolesAsync(string userId, CancellationToken cancellationToken);
    Task<IResponse> UpdateUserRoleAsync(string userId, UpdateUserRoleRequest request);
    Task<IResponse> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<IResponse> ResetPasswordAsync(string userId, ResetPasswordRequest request);
    Task<IResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request);
}
