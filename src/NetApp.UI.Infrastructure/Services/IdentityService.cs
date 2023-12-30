using Microsoft.Extensions.Localization;
using NetApp.Shared;
using System.Net.Http.Json;

namespace NetApp.UI.Infrastructure.Services;
public class IdentityService(HttpClient httpClient, ISnackbar snackbar, IStringLocalizer<NetAppLocalizer> localizer) : BaseService(httpClient, snackbar, localizer), IIdentityService
{
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

    public async Task<IResponse<UserRolesResponse>> GetRolesAsync(string userId, CancellationToken cancellation)
    {
        return await GetAsync<Response<UserRolesResponse>>(Endpoints.Identity.UserRoles(userId), cancellation);
    }

    public async Task<UserDto?> GetUserAsync(string id) =>
    await GetAsync<UserDto>(Endpoints.Identity.UserById(id));

    public async Task<PaginatedResponse<UserDto>> GetUsersAsync(TableState state, string? searchString, CancellationToken cancellationToken) =>
    await GetAsync<PaginatedResponse<UserDto>>(Endpoints.Identity.Users, cancellationToken);

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

    public async Task<IResponse> UpdatePasswordAsync(string userId, UpdatePasswordRequest request)
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
