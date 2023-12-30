using System.Security.Claims;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using NetApp.Shared;
using NetApp.UI.Infrastructure.Extensions;

namespace NetApp.UI.Infrastructure.Services;

internal class AuthenticationService(HttpClient httpClient,
    AuthenticationStateProvider authenticationStateProvider,
    IStorageService storageService, 
    ISnackbar snackbar, 
    IStringLocalizer<NetAppLocalizer> localizer) : BaseService(httpClient, snackbar, localizer), IAuthenticationService
{
    public async Task<ClaimsPrincipal> GetCurrentUser()
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User;
    }

    public async Task<IResponse> LoginAsync(AuthenticationRequest request)
    {
        var response = await PostAsync<AuthenticationRequest, AuthenticationResponse>(Endpoints.Identity.Login, request);
        if (!response.Succeeded) return response;
        await ((NetAppAuthStateProvider)authenticationStateProvider).NotifyAuthenticatedAsync(response.Data!);
        return response;
    }

    public async Task LogoutAsync()
    {
        await ((NetAppAuthStateProvider)authenticationStateProvider).NotifyLogoutAsync();
    }

    public async Task TryRefreshToken(bool force = false)
    {
        try
        {
            if (force)
            {
                await RefreshToken();
                return;
            }

            var currentAuthState = await ((NetAppAuthStateProvider)authenticationStateProvider).GetAuthenticationStateAsync();
            if (!currentAuthState.IsAnonymous() && currentAuthState.ShouldRefreshToken())
            {
                //Console.WriteLine($" TryRefreshToken is called by: {(new System.Diagnostics.StackTrace())!.GetFrame(1)!.GetMethod()!.Module.Name}");
                await RefreshToken();
            }
        }
        catch
        {
            if (force) throw;
        }
    }
    private async Task RefreshToken()
    {
        var refreshToken = await storageService.GetItemAsync<string>(ApplicationConstants.Storage.RefreshToken);
        var token = await storageService.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken);
        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(token)) return;
        var request = new RefreshTokenRequest
        {
            RefreshToken = refreshToken,
            Token = token
        };
        var response = await PostAsync<RefreshTokenRequest, AuthenticationResponse>(Endpoints.Identity.RefreshToken, request);
        if (!response!.Succeeded) return;

        await ((NetAppAuthStateProvider)authenticationStateProvider).NotifyAuthenticatedAsync(response!.Data!);
    }
}