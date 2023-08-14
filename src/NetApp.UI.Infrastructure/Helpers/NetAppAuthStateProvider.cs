using System.Net.Http.Headers;
using System.Security.Claims;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using NetApp.UI.Infrastructure.Extensions;
using NetApp.UI.Infrastructure.Store;

namespace NetApp.UI.Infrastructure;
public class NetAppAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IStorageService _storageService;
    private readonly ISnackbar _snackbar;
    private readonly IDispatcher _dispatcher;
    private readonly IState<NetAppState> _appState;
    public NetAppAuthStateProvider(IStorageService storageService, ISnackbar snackbar, HttpClient httpClient, IDispatcher dispatcher, IState<NetAppState> appState)
    {
        _storageService = storageService;
        _snackbar = snackbar;
        _httpClient = httpClient;
        _dispatcher = dispatcher;
        _appState = appState;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _storageService.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken);
        Console.WriteLine($"NetAppAuthStateProvider.GetAuthenticationStateAsync: {_appState.Value.AuthToken}");
        if (string.IsNullOrWhiteSpace(_appState.Value.AuthToken))
            return AuthenticationStateExtensions.Anonymous;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _appState.Value.AuthToken!);
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(GetClaims(_appState.Value.AuthToken!), "jwt"));
        return new AuthenticationState(claimsPrincipal);
    }

    public void  NotifyAuthenticated(AuthenticationResponse response)
    {
        // await _storageService.SetItemAsync(ApplicationConstants.Storage.AuthToken, response.JWToken);
        // await _storageService.SetItemAsync(ApplicationConstants.Storage.RefreshToken, response.RefreshToken);
        _dispatcher.Dispatch(new SetAuthTokenAction(response.JWToken, response.RefreshToken));
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task NotifyLogoutAsync()
    {
        _dispatcher.Dispatch(new SetAuthTokenAction("", ""));
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
