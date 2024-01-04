using System.Net.Http.Headers;
using System.Security.Claims;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NetApp.UI.Infrastructure.Extensions;
using NetApp.UI.Infrastructure.Store;

namespace NetApp.UI.Infrastructure;
public class NetAppAuthStateProvider(IStorageService storageService, HttpClient httpClient) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await storageService.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken);
        if (string.IsNullOrWhiteSpace(token))
            return AuthenticationStateExtensions.Anonymous;
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(StringExtensions.GetClaims(token), "jwt"));
        var authenticationState = new AuthenticationState(claimsPrincipal);
        if (authenticationState.IsTokenExpired())
            return AuthenticationStateExtensions.Anonymous;
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        return authenticationState;
    }

    public async Task NotifyAuthenticatedAsync(AuthenticationResponse response)
    {
        await storageService.SetItemAsync(ApplicationConstants.Storage.AuthToken, response.JWToken);
        await storageService.SetItemAsync(ApplicationConstants.Storage.RefreshToken, response.RefreshToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task NotifyLogoutAsync()
    {
        await storageService.ClearAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(AuthenticationStateExtensions.Anonymous));
    }






}
