using Fluxor;
using Microsoft.AspNetCore.Components;
using NetApp.UI.Infrastructure.Store;

namespace NetApp.Pwa.Shared;

public partial class AuthorizedLayout
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


    private void ToggleTheme(bool isDarkMode) => Dispatcher.ChangeThemeMode(isDarkMode);

}