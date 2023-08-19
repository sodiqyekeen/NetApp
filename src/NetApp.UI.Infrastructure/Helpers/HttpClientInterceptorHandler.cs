using Microsoft.AspNetCore.Components.Authorization;
using NetApp.UI.Infrastructure.Services;
using System.Net;
using Toolbelt.Blazor;

namespace NetApp.UI.Infrastructure;
public class HttpClientInterceptorHandler
{
    private readonly HttpClientInterceptor _interceptor;
    private readonly ISnackbar _snackbar;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IAuthenticationService _authenticationService;
    private static readonly string[] _exemptedPath = { "login", "refresh", "logout", "netapphub" };


    public HttpClientInterceptorHandler(HttpClientInterceptor interceptor,
        ISnackbar snackbar,
        AuthenticationStateProvider authenticationStateProvider,
        IAuthenticationService authenticationService)
    {
        _interceptor = interceptor;
        _snackbar = snackbar;
        _authenticationStateProvider = authenticationStateProvider;
        _authenticationService = authenticationService;
    }

    public void RegisterEvent()
    {
        _interceptor.BeforeSendAsync += InterceptBeforeSendingRequest;
        _interceptor.AfterSendAsync += InterceptResponseAsync;
    }

    private async Task InterceptBeforeSendingRequest(object sender, HttpClientInterceptorEventArgs e)
    {
        if (_exemptedPath.Any(x => e.Request!.RequestUri!.AbsoluteUri.Contains(x)))
            return;

        try
        {
            await _authenticationService.TryRefreshToken();
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine(ex.Message);
#endif
            await _authenticationService.LogoutAsync();
        }
    }

    private async Task InterceptResponseAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        if (e?.Response?.StatusCode is not HttpStatusCode.Unauthorized)
            return;

        _snackbar.Add(ApplicationConstants.ErrorMessages.SessionTimeout, Severity.Error);
        await ((NetAppAuthStateProvider)_authenticationStateProvider).NotifyLogoutAsync();
    }

    public void DisposeEvent()
    {
        _interceptor.BeforeSendAsync -= InterceptBeforeSendingRequest;
        _interceptor.AfterSendAsync -= InterceptResponseAsync;
    }
}
