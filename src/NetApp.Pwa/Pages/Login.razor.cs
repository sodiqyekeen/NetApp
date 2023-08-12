using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using NetApp.Dtos;
using NetApp.UI.Infrastructure;

namespace NetApp.Pwa.Pages;

public partial class Login
{
    [Inject] private IStorageService StorageService { get; set; } = null!;
    AuthenticationRequest loginModel = new();
    bool passwordVisibility;
    string passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    InputType passwordInputType = InputType.Password;
    AuthenticationState? authenticationState;

    protected override async Task OnInitializedAsync()
    {
        await StorageService.ClearAsync();
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