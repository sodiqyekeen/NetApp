using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NetApp.Dtos;
using NetApp.UI.Infrastructure;
using NetApp.UI.Infrastructure.Extensions;
using NetApp.UI.Infrastructure.Services;
using NetApp.UI.Infrastructure.Store;

namespace NetApp.Pwa.Pages.Auth;

public partial class Login
{
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = null!;
    [Inject] private IDispatcher Dispatcher { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NetAppAuthStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    AuthenticationRequest loginModel = new();
   
    protected override async Task OnInitializedAsync()
    {
        var authenticationState = await AuthStateProvider.GetAuthenticationStateAsync();
        if (authenticationState.IsAnonymous())
        {
            await AuthStateProvider.NotifyLogoutAsync();
        }
        else
        {
            NavigationManager.NavigateTo(ApplicationConstants.Routes.Dashboard);
        }
    }

    async void LoginAsync()
    {
        Dispatcher.ToggleLoader(true);
        var response = await AuthenticationService.LoginAsync(loginModel);
        Dispatcher.ToggleLoader(false);
        if (!response.Succeeded)
            Snackbar.Add(response.Message, Severity.Error);
    }
   
}