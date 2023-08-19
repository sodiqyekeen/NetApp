using System.Net.Http.Headers;
using System.Security.Claims;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NetApp.UI.Infrastructure.Extensions;
using NetApp.UI.Infrastructure.Store;

namespace NetApp.UI.Infrastructure;
public class NetAppAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IStorageService _storageService;
    private readonly ISnackbar _snackbar;
    private readonly NavigationManager _navigationManager;
    public NetAppAuthStateProvider(IStorageService storageService, ISnackbar snackbar, HttpClient httpClient, NavigationManager navigationManager)
    {
        _storageService = storageService;
        _snackbar = snackbar;
        _httpClient = httpClient;
        _navigationManager = navigationManager;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _storageService.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken);
        if (string.IsNullOrWhiteSpace(token))
            return AuthenticationStateExtensions.Anonymous;
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(GetClaims(token), "jwt"));
        var authenticationState = new AuthenticationState(claimsPrincipal);
        if(authenticationState.IsTokenExpired())
            return AuthenticationStateExtensions.Anonymous;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        return authenticationState;
    }

    public async Task NotifyAuthenticatedAsync(AuthenticationResponse response)
    {
        await _storageService.SetItemAsync(ApplicationConstants.Storage.AuthToken, response.JWToken);
        await _storageService.SetItemAsync(ApplicationConstants.Storage.RefreshToken, response.RefreshToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task NotifyLogoutAsync()
    {
        await _storageService.ClearAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(AuthenticationStateExtensions.Anonymous));
    }

    public async Task ValidateSessionAsync()
    {
        var currentAuthState = await GetAuthenticationStateAsync();
        if (currentAuthState.IsAnonymous() || currentAuthState.IsTokenExpired())
        {
            _snackbar.Add(ApplicationConstants.ErrorMessages.SessionTimeout, Severity.Error);
            await NotifyLogoutAsync();
        }
    }

    private static List<Claim> GetClaims(string jwtToken)
    {
        string payload = jwtToken.Split(".")[1];
        byte[] jsonBytes = payload.ParseBase64StringWithoutPadding();
        var keyValuePairs = jsonBytes.FromBytes<Dictionary<string, object>>();
        var claims = keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)).ToList();
        return claims;
    }




}
