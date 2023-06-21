using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using NetApp.Extensions;
using NetApp.UI.Infrastructure.Extensions;

namespace NetApp.UI.Infrastructure;
public class NetAppAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IStorageService _storageService;
    private readonly ISnackbar _snackbar;
    public NetAppAuthStateProvider(IStorageService storageService, ISnackbar snackbar, HttpClient httpClient)
    {
        _storageService = storageService;
        _snackbar = snackbar;
        _httpClient = httpClient;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _storageService.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken);
        if (string.IsNullOrEmpty(token))
        {
            return AuthenticationStateExtensions.Anonymous;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(GetClaims(token), "jwt"));
       // _revliogStateManager.CurrentUsername = claimsPrincipal.FindFirst("username")!.Value;
        return new AuthenticationState(claimsPrincipal);
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

    public async Task ValidateSession()
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
        byte[] jsonBytes = ParseBase64StringWithoutPadding(payload);
        var keyValuePairs = jsonBytes.FromBytes<Dictionary<string, object>>();
        var claims = keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)).ToList();
        return claims;
    }

    private static byte[] ParseBase64StringWithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }
        return Convert.FromBase64String(base64);
    }
}
