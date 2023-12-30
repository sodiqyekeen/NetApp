using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using NetApp.Extensions;
using NetApp.UI.Infrastructure;
using NetApp.UI.Infrastructure.Extensions;
using NetApp.UI.Infrastructure.Services;
using NetApp.UI.Infrastructure.Store;

namespace NetApp.Pwa.Shared;

public partial class AuthorizedLayout : IDisposable
{
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;

    private bool _showDrawer = true;
    private void ToggleDrawer()
    {
        _showDrawer = !_showDrawer;
    }
    [Inject] private IState<NetAppState> AppState { get; set; } = null!;
    [Inject] private IDispatcher Dispatcher { get; set; } = null!;
    [Inject] private IState<Fluxor.Blazor.Web.Middlewares.Routing.RoutingState> RoutingState { get; set; } = null!;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private NetAppAuthStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = null!;
         
    private HashSet<string> _anonymousPages = new()
    {
        "login",
        "register",
        "forgot-password",
        "reset-password"
    };

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += HandleLocationChanged;
    }
    private async void Logout()
    {
        await AuthenticationService.LogoutAsync();
    }

    private async void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (_anonymousPages.Contains(e.Location.GetCurrentPageFromUri())) return;
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        if (!authState.IsAnonymous()) return;
        await AuthStateProvider.NotifyLogoutAsync();
        NavigationManager.NavigateTo(ApplicationConstants.Routes.Login, true);
    }


    public void Dispose() => NavigationManager.LocationChanged -= HandleLocationChanged;

    private void ToggleTheme(bool isDarkMode) => Dispatcher.ChangeThemeMode(isDarkMode);

}