using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NetApp.Models;
using NetApp.UI.Infrastructure.Extensions;
namespace NetApp.Pwa.Dialogs;
public class DialogBase : ComponentBase
{

    [Inject] protected IDispatcher Dispatcher { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;

    [CascadingParameter]
    public MudDialogInstance? Dialog { get; set; }

    protected void Cancel()
    {
        Dialog?.Cancel();
    }

    protected async Task SubmitAsync(Func<Task<IResponse<string>>> saveFunction)
    {
        Dispatcher.ToggleLoader(true);
        var response = await saveFunction();
        Dispatcher.ToggleLoader(false);
        if (response.Succeeded)
        {
            Snackbar.Add(response.Message, Severity.Success);
            Dialog?.Close();
        }
        else
        {
            Snackbar.Add(response.Message, Severity.Error);
        }
    }
}