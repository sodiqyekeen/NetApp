using System.Net.Http.Json;

namespace NetApp.UI.Infrastructure.Services;
public class IdentityService : BaseService, IIdentityService
{
    public IdentityService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<IResponse> ConfirmEmailAsync(string userId, string code)
    {
        var response = await _httpClient.PostAsync(Endpoints.Identity.ConfirmEmail(userId, code), null);
        return (await response.Content.ReadFromJsonAsync<IResponse>())!;
    }

    public async Task<IResponse> DeleteUserAsync(string userId)
    {
        return await DeleteAsync(Endpoints.Identity.UserById(userId));
    }

    public async Task<IResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        return await PostAsync(Endpoints.Identity.ForgotPassword, request);
    }

    public async Task<IResponse<UserRolesResponse>> GetRolesAsync(string userId)
    {
        return await GetAsync<IResponse<UserRolesResponse>>(Endpoints.Identity.UserRoles(userId));
    }

    public async Task<PaginatedResponse<User>> GetUsersAsync(TableState state, string? searchString)
    {
        var response = await GetAsync<IResponse<PaginatedResponse<User>>>(Endpoints.Identity.Users);
        return response!.Data!;
    }

    public async Task<IResponse<string>> RegisterUserAsync(RegisterRequest request)
    {
        return await PostAsync<RegisterRequest, string>(Endpoints.Identity.Users, request);
    }

    public async Task<IResponse> ResendConfirmationMailAsync(ConfirmationMailRequest request)
    {
        return await PostAsync(Endpoints.Identity.ResendConfirmationEmail(request.UserId), request);
    }

    public async Task<IResponse> ResetPasswordAsync(string userId, ResetPasswordRequest request)
    {
        return await PostAsync(Endpoints.Identity.ResetPassword(userId), request);
    }

    public async Task<IResponse> UpdatePasswordAsync(string userId,UpdatePasswordRequest request)
    {
        return await PutAsync(Endpoints.Identity.UpdatePassword(userId), request);
    }

    public async Task<IResponse> UpdateUserAsync(string userId, EditUserRequest request)
    {
        return await PutAsync(Endpoints.Identity.UserById(userId), request);
    }

    public async Task<IResponse> UpdateUserRoleAsync(string userId, UpdateUserRoleRequest request)
    {
        return await PutAsync(Endpoints.Identity.UserRoles(userId), request);
    }
}
