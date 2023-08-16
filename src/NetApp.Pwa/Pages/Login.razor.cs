using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using NetApp.Dtos;
using NetApp.UI.Infrastructure;
using NetApp.UI.Infrastructure.Services;
using NetApp.UI.Infrastructure.Store;
using NetApp.UI.Infrastructure.Extensions;
using NetApp.Extensions;

namespace NetApp.Pwa.Pages;

public partial class Login
{
    [Inject] private IStorageService StorageService { get; set; } = null!;
    [Inject] private IAuthenticationService AuthenticationService { get; set; } = null!;
    [Inject] private IDispatcher Dispatcher { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NetAppAuthStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    AuthenticationRequest loginModel = new();
    bool passwordVisibility;
    string passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    InputType passwordInputType = InputType.Password;
    AuthenticationState? authenticationState;

    protected override async Task OnInitializedAsync()
    {
        authenticationState = await AuthStateProvider.GetAuthenticationStateAsync();
        if (authenticationState.IsAnonymous())
        {
            Console.WriteLine("Anonymous");
            await AuthStateProvider.NotifyLogoutAsync();
            return;
        }
        Console.WriteLine("Authenticated.");
        NavigationManager.NavigateTo("/");
    }

    async void LoginAsync()
    {
        Dispatcher.ToggleLoader(true);
        var response = await AuthenticationService.LoginAsync(loginModel);
        Dispatcher.ToggleLoader(false);
        if (!response.Succeeded)
            Snackbar.Add(response.Message, Severity.Error);
    }
    void TogglePasswordVisibility()
    {
        if (passwordVisibility)
        {
            passwordVisibility = false;
            passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            passwordInputType = InputType.Password;
        }
        else
        {
            passwordVisibility = true;
            passwordInputIcon = Icons.Material.Filled.Visibility;
            passwordInputType = InputType.Text;
        }
    }
}