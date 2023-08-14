using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using NetApp.UI.Infrastructure.Extensions;

namespace NetApp.UI.Infrastructure.Services;

internal class AuthenticationService : BaseService, IAuthenticationService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IStorageService _storageService;
    public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, IStorageService storageService) : base(httpClient)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _storageService = storageService;
    }

    public async Task<ClaimsPrincipal> GetCurrentUser()
    {
        var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User;
    }

    public async Task<IResponse> LoginAsync(AuthenticationRequest request)
    {
        var response = await PostAsync<AuthenticationRequest, AuthenticationResponse>(Endpoints.Identity.Login, request);
        if (!response.Succeeded) return response;
        ((NetAppAuthStateProvider)_authenticationStateProvider).NotifyAuthenticated(response.Data!);
        return response;
    }

    public async Task LogoutAsync()
    {
        await ((NetAppAuthStateProvider)_authenticationStateProvider).NotifyLogoutAsync();
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

            var currentAuthState = await ((NetAppAuthStateProvider)_authenticationStateProvider).GetAuthenticationStateAsync();
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
        var refreshToken = await _storageService.GetItemAsync<string>(ApplicationConstants.Storage.RefreshToken);
        var token = await _storageService.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken);
        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(token)) return;
        var request = new RefreshTokenRequest
        {
            RefreshToken = refreshToken,
            Token = token
        };
        var response = await PostAsync<RefreshTokenRequest, AuthenticationResponse>(Endpoints.Identity.RefreshToken, request);
        if (!response!.Succeeded) return;

        ((NetAppAuthStateProvider)_authenticationStateProvider).NotifyAuthenticated(response!.Data!);
    }
}