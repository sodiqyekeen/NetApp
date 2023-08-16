using Moq;
using MudBlazor;
using NetApp.Dtos;
using NetApp.UI.Infrastructure;
using System.Net.Http.Headers;
using System.Security.Claims;
using NetApp.Extensions;

namespace NetApp.Tests.UI;



[Collection("Sequential")]
public class NetAppAuthStateProviderTests
{
    private readonly NetAppAuthStateProvider _authStateProvider;
    private readonly Mock<IStorageService> _storageService;
    private readonly Mock<ISnackbar> _snackbar;
    private readonly Mock<HttpClient> _httpClient;

    public NetAppAuthStateProviderTests()
    {
        _storageService = new Mock<IStorageService>();
        _snackbar = new Mock<ISnackbar>();
        _httpClient = new Mock<HttpClient>(MockBehavior.Default);

        _authStateProvider = new NetAppAuthStateProvider(_storageService.Object, _snackbar.Object, _httpClient.Object);
    }

    [Fact(DisplayName = "GetAuthenticationStateAsync when token is null or empty return anonymous authenticationstate")]
    public async Task GetAuthenticationStateAsync_TokenNullOrEmpty_ReturnsAnonymousAuthenticationState()
    {
        _storageService.Setup(x => x.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken)).ReturnsAsync((string)null);
        var result = await _authStateProvider.GetAuthenticationStateAsync();
        Assert.NotNull(result);
        Assert.True(result.User.Identity?.IsAuthenticated == false);
    }

    [Fact(DisplayName = "GetAuthenticationStateAsync when token is not null or empty return authenticated authenticationstate")]
    public async Task GetAuthenticationStateAsync_TokenNotNullOrEmpty_ReturnsAuthenticatedAuthenticationState()
    {
        _storageService.Setup(x => x.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken)).ReturnsAsync("dummyToken");
        //_httpClient.Setup(x => x.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", "dummyToken"));
        var result = await _authStateProvider.GetAuthenticationStateAsync();
        Assert.True(result.User.Identity?.IsAuthenticated == true);
        Assert.IsAssignableFrom<ClaimsPrincipal>(result.User);
    }

    [Fact(DisplayName = "NotifyAuthenticatedAsync sets AuthToken and returns notified state")]
    public async Task NotifyAuthenticatedAsync_SetAuthTokenAndReturnNotifiedState()
    {
        var response = new AuthenticationResponse("dummyToken", "refresh");
        await _authStateProvider.NotifyAuthenticatedAsync(response);

        _storageService.Verify(x => x.SetItemAsync(ApplicationConstants.Storage.AuthToken, response.JWToken), Times.Once());
        _storageService.Verify(x => x.SetItemAsync(ApplicationConstants.Storage.RefreshToken, response.RefreshToken), Times.Once());
    }

    [Fact(DisplayName = "NotifyLogoutAsync clears storage and return anonymous authenticationstate")]
    public async Task NotifyLogoutAsync_ClearsStorageAndReturnAnonymousAuthenticationState()
    {
        await _authStateProvider.NotifyLogoutAsync();

        _storageService.Verify(x => x.ClearAsync(), Times.Once());
        var result = await _authStateProvider.GetAuthenticationStateAsync();
        Assert.NotNull(result);
        Assert.True(result.User.Identity?.IsAuthenticated == false);
    }

    [Fact(DisplayName = "ValidateSession returns User Not Authenticated when Token is null or empty")]
    public async Task ValidateSession_ReturnsUserNotAuthenticatedWhenTokenIsNullOrEmpty()
    {
        _storageService.Setup(x => x.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken)).ReturnsAsync((string)null);
        await _authStateProvider.ValidateSession();
        // _snackbar.Verify(x => x.Add(ApplicationConstants.ErrorMessages.SessionTimeout, It.IsAny<Severity>()), Times.Once);
    }

    [Fact(DisplayName = "ValidateSession notifies Logout when Current Authentication State is Anonymous")]
    public async Task ValidateSession_NotifiesLogoutWhenCurrentAuthenticationStateIsAnonymous()
    {
        var mockIdentity = new Mock<ClaimsIdentity>();
        var mockPrincipal = new Mock<ClaimsPrincipal>();
        mockPrincipal.Setup(x => x.Identity).Returns(mockIdentity.Object);
        _storageService.Setup(x => x.GetItemAsync<string>(ApplicationConstants.Storage.AuthToken)).ReturnsAsync("dummyToken");
        //  _httpClient.Setup(x => x.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", "dummyToken"));
        var result = await _authStateProvider.GetAuthenticationStateAsync();
        Assert.IsAssignableFrom<ClaimsPrincipal>(result.User);
        await _authStateProvider.ValidateSession();
        // _snackbar.Verify(x => x.Add(ApplicationConstants.ErrorMessages.SessionTimeout, It.IsAny<Severity>()), Times.Once);
        _storageService.Verify(x => x.ClearAsync(), Times.Once());
        Assert.NotNull(result);
        Assert.True(result.User.Identity?.IsAuthenticated == false);
    }

    [Fact(DisplayName = "Should get claims from JWT token")]
    public void ShouldGetClaimsFromJwtToken()
    {
        const string jwtToken = "token.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ==.sign";
        var expectedClaims = new List<Claim>
            {
                new Claim("sub", "1234567890"),
                new Claim("name", "John Doe"),
                new Claim("iat", "1516239022")
            };

        var claims = GenerateClaimsFromToken(jwtToken);
        Assert.Equal(expectedClaims.Count, claims.Count);
        foreach (var claim in claims)
        {
            Assert.Contains(expectedClaims, c => c.Value == claim.Value && c.Type == claim.Type);
        }
    }

    private static List<Claim> GenerateClaimsFromToken(string jwtToken)
    {
        byte[] jsonBytes = jwtToken.Split(".")[1].ParseBase64StringWithoutPadding();
        var keyValuePairs = jsonBytes.FromBytes<Dictionary<string, object>>();
        var claims = keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)).ToList();
        return claims;
    }
}
